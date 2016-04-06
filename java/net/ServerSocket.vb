Imports Microsoft.VisualBasic
Imports System
Imports System.Runtime.CompilerServices

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

Namespace java.net


	''' <summary>
	''' This class implements server sockets. A server socket waits for
	''' requests to come in over the network. It performs some operation
	''' based on that request, and then possibly returns a result to the requester.
	''' <p>
	''' The actual work of the server socket is performed by an instance
	''' of the {@code SocketImpl} class. An application can
	''' change the socket factory that creates the socket
	''' implementation to configure itself to create sockets
	''' appropriate to the local firewall.
	''' 
	''' @author  unascribed </summary>
	''' <seealso cref=     java.net.SocketImpl </seealso>
	''' <seealso cref=     java.net.ServerSocket#setSocketFactory(java.net.SocketImplFactory) </seealso>
	''' <seealso cref=     java.nio.channels.ServerSocketChannel
	''' @since   JDK1.0 </seealso>
	Public Class ServerSocket
		Implements java.io.Closeable

		''' <summary>
		''' Various states of this socket.
		''' </summary>
		Private created As Boolean = False
		Private bound As Boolean = False
		Private closed As Boolean = False
		Private closeLock As New Object

		''' <summary>
		''' The implementation of this Socket.
		''' </summary>
		Private impl As SocketImpl

		''' <summary>
		''' Are we using an older SocketImpl?
		''' </summary>
		Private oldImpl As Boolean = False

		''' <summary>
		''' Package-private constructor to create a ServerSocket associated with
		''' the given SocketImpl.
		''' </summary>
		Friend Sub New(  impl As SocketImpl)
			Me.impl = impl
			impl.serverSocket = Me
		End Sub

		''' <summary>
		''' Creates an unbound server socket.
		''' </summary>
		''' <exception cref="IOException"> IO error when opening the socket.
		''' @revised 1.4 </exception>
		Public Sub New()
			implmpl()
		End Sub

		''' <summary>
		''' Creates a server socket, bound to the specified port. A port number
		''' of {@code 0} means that the port number is automatically
		''' allocated, typically from an ephemeral port range. This port
		''' number can then be retrieved by calling <seealso cref="#getLocalPort getLocalPort"/>.
		''' <p>
		''' The maximum queue length for incoming connection indications (a
		''' request to connect) is set to {@code 50}. If a connection
		''' indication arrives when the queue is full, the connection is refused.
		''' <p>
		''' If the application has specified a server socket factory, that
		''' factory's {@code createSocketImpl} method is called to create
		''' the actual socket implementation. Otherwise a "plain" socket is created.
		''' <p>
		''' If there is a security manager,
		''' its {@code checkListen} method is called
		''' with the {@code port} argument
		''' as its argument to ensure the operation is allowed.
		''' This could result in a SecurityException.
		''' 
		''' </summary>
		''' <param name="port">  the port number, or {@code 0} to use a port
		'''                   number that is automatically allocated.
		''' </param>
		''' <exception cref="IOException">  if an I/O error occurs when opening the socket. </exception>
		''' <exception cref="SecurityException">
		''' if a security manager exists and its {@code checkListen}
		''' method doesn't allow the operation. </exception>
		''' <exception cref="IllegalArgumentException"> if the port parameter is outside
		'''             the specified range of valid port values, which is between
		'''             0 and 65535, inclusive.
		''' </exception>
		''' <seealso cref=        java.net.SocketImpl </seealso>
		''' <seealso cref=        java.net.SocketImplFactory#createSocketImpl() </seealso>
		''' <seealso cref=        java.net.ServerSocket#setSocketFactory(java.net.SocketImplFactory) </seealso>
		''' <seealso cref=        SecurityManager#checkListen </seealso>
		Public Sub New(  port As Integer)
			Me.New(port, 50, Nothing)
		End Sub

		''' <summary>
		''' Creates a server socket and binds it to the specified local port
		''' number, with the specified backlog.
		''' A port number of {@code 0} means that the port number is
		''' automatically allocated, typically from an ephemeral port range.
		''' This port number can then be retrieved by calling
		''' <seealso cref="#getLocalPort getLocalPort"/>.
		''' <p>
		''' The maximum queue length for incoming connection indications (a
		''' request to connect) is set to the {@code backlog} parameter. If
		''' a connection indication arrives when the queue is full, the
		''' connection is refused.
		''' <p>
		''' If the application has specified a server socket factory, that
		''' factory's {@code createSocketImpl} method is called to create
		''' the actual socket implementation. Otherwise a "plain" socket is created.
		''' <p>
		''' If there is a security manager,
		''' its {@code checkListen} method is called
		''' with the {@code port} argument
		''' as its argument to ensure the operation is allowed.
		''' This could result in a SecurityException.
		''' 
		''' The {@code backlog} argument is the requested maximum number of
		''' pending connections on the socket. Its exact semantics are implementation
		''' specific. In particular, an implementation may impose a maximum length
		''' or may choose to ignore the parameter altogther. The value provided
		''' should be greater than {@code 0}. If it is less than or equal to
		''' {@code 0}, then an implementation specific default will be used.
		''' <P>
		''' </summary>
		''' <param name="port">     the port number, or {@code 0} to use a port
		'''                      number that is automatically allocated. </param>
		''' <param name="backlog">  requested maximum length of the queue of incoming
		'''                      connections.
		''' </param>
		''' <exception cref="IOException">  if an I/O error occurs when opening the socket. </exception>
		''' <exception cref="SecurityException">
		''' if a security manager exists and its {@code checkListen}
		''' method doesn't allow the operation. </exception>
		''' <exception cref="IllegalArgumentException"> if the port parameter is outside
		'''             the specified range of valid port values, which is between
		'''             0 and 65535, inclusive.
		''' </exception>
		''' <seealso cref=        java.net.SocketImpl </seealso>
		''' <seealso cref=        java.net.SocketImplFactory#createSocketImpl() </seealso>
		''' <seealso cref=        java.net.ServerSocket#setSocketFactory(java.net.SocketImplFactory) </seealso>
		''' <seealso cref=        SecurityManager#checkListen </seealso>
		Public Sub New(  port As Integer,   backlog As Integer)
			Me.New(port, backlog, Nothing)
		End Sub

		''' <summary>
		''' Create a server with the specified port, listen backlog, and
		''' local IP address to bind to.  The <i>bindAddr</i> argument
		''' can be used on a multi-homed host for a ServerSocket that
		''' will only accept connect requests to one of its addresses.
		''' If <i>bindAddr</i> is null, it will default accepting
		''' connections on any/all local addresses.
		''' The port must be between 0 and 65535, inclusive.
		''' A port number of {@code 0} means that the port number is
		''' automatically allocated, typically from an ephemeral port range.
		''' This port number can then be retrieved by calling
		''' <seealso cref="#getLocalPort getLocalPort"/>.
		''' 
		''' <P>If there is a security manager, this method
		''' calls its {@code checkListen} method
		''' with the {@code port} argument
		''' as its argument to ensure the operation is allowed.
		''' This could result in a SecurityException.
		''' 
		''' The {@code backlog} argument is the requested maximum number of
		''' pending connections on the socket. Its exact semantics are implementation
		''' specific. In particular, an implementation may impose a maximum length
		''' or may choose to ignore the parameter altogther. The value provided
		''' should be greater than {@code 0}. If it is less than or equal to
		''' {@code 0}, then an implementation specific default will be used.
		''' <P> </summary>
		''' <param name="port">  the port number, or {@code 0} to use a port
		'''              number that is automatically allocated. </param>
		''' <param name="backlog"> requested maximum length of the queue of incoming
		'''                connections. </param>
		''' <param name="bindAddr"> the local InetAddress the server will bind to
		''' </param>
		''' <exception cref="SecurityException"> if a security manager exists and
		''' its {@code checkListen} method doesn't allow the operation.
		''' </exception>
		''' <exception cref="IOException"> if an I/O error occurs when opening the socket. </exception>
		''' <exception cref="IllegalArgumentException"> if the port parameter is outside
		'''             the specified range of valid port values, which is between
		'''             0 and 65535, inclusive.
		''' </exception>
		''' <seealso cref= SocketOptions </seealso>
		''' <seealso cref= SocketImpl </seealso>
		''' <seealso cref= SecurityManager#checkListen
		''' @since   JDK1.1 </seealso>
		Public Sub New(  port As Integer,   backlog As Integer,   bindAddr As InetAddress)
			implmpl()
			If port < 0 OrElse port > &HFFFF Then Throw New IllegalArgumentException("Port value out of range: " & port)
			If backlog < 1 Then backlog = 50
			Try
				bind(New InetSocketAddress(bindAddr, port), backlog)
			Catch e As SecurityException
				close()
				Throw e
			Catch e As java.io.IOException
				close()
				Throw e
			End Try
		End Sub

		''' <summary>
		''' Get the {@code SocketImpl} attached to this socket, creating
		''' it if necessary.
		''' </summary>
		''' <returns>  the {@code SocketImpl} attached to that ServerSocket. </returns>
		''' <exception cref="SocketException"> if creation fails.
		''' @since 1.4 </exception>
		Friend Overridable Property impl As SocketImpl
			Get
				If Not created Then createImpl()
				Return impl
			End Get
		End Property

		Private Sub checkOldImpl()
			If impl Is Nothing Then Return
			' SocketImpl.connect() is a protected method, therefore we need to use
			' getDeclaredMethod, therefore we need permission to access the member
			Try
				java.security.AccessController.doPrivileged(New PrivilegedExceptionActionAnonymousInnerClassHelper(Of T)
			Catch e As java.security.PrivilegedActionException
				oldImpl = True
			End Try
		End Sub

		Private Class PrivilegedExceptionActionAnonymousInnerClassHelper(Of T)
			Implements java.security.PrivilegedExceptionAction(Of T)

			Public Overridable Function run() As Void
				outerInstance.impl.GetType().getDeclaredMethod("connect", GetType(SocketAddress), GetType(Integer))
				Return Nothing
			End Function
		End Class

		Private Sub setImpl()
			If factory IsNot Nothing Then
				impl = factory.createSocketImpl()
				checkOldImpl()
			Else
				' No need to do a checkOldImpl() here, we know it's an up to date
				' SocketImpl!
				impl = New SocksSocketImpl
			End If
			If impl IsNot Nothing Then impl.serverSocket = Me
		End Sub

		''' <summary>
		''' Creates the socket implementation.
		''' </summary>
		''' <exception cref="IOException"> if creation fails
		''' @since 1.4 </exception>
		Friend Overridable Sub createImpl()
			If impl Is Nothing Then implmpl()
			Try
				impl.create(True)
				created = True
			Catch e As java.io.IOException
				Throw New SocketException(e.Message)
			End Try
		End Sub

		''' 
		''' <summary>
		''' Binds the {@code ServerSocket} to a specific address
		''' (IP address and port number).
		''' <p>
		''' If the address is {@code null}, then the system will pick up
		''' an ephemeral port and a valid local address to bind the socket.
		''' <p> </summary>
		''' <param name="endpoint">        The IP address and port number to bind to. </param>
		''' <exception cref="IOException"> if the bind operation fails, or if the socket
		'''                     is already bound. </exception>
		''' <exception cref="SecurityException">       if a {@code SecurityManager} is present and
		''' its {@code checkListen} method doesn't allow the operation. </exception>
		''' <exception cref="IllegalArgumentException"> if endpoint is a
		'''          SocketAddress subclass not supported by this socket
		''' @since 1.4 </exception>
		Public Overridable Sub bind(  endpoint As SocketAddress)
			bind(endpoint, 50)
		End Sub

		''' 
		''' <summary>
		''' Binds the {@code ServerSocket} to a specific address
		''' (IP address and port number).
		''' <p>
		''' If the address is {@code null}, then the system will pick up
		''' an ephemeral port and a valid local address to bind the socket.
		''' <P>
		''' The {@code backlog} argument is the requested maximum number of
		''' pending connections on the socket. Its exact semantics are implementation
		''' specific. In particular, an implementation may impose a maximum length
		''' or may choose to ignore the parameter altogther. The value provided
		''' should be greater than {@code 0}. If it is less than or equal to
		''' {@code 0}, then an implementation specific default will be used. </summary>
		''' <param name="endpoint">        The IP address and port number to bind to. </param>
		''' <param name="backlog">         requested maximum length of the queue of
		'''                          incoming connections. </param>
		''' <exception cref="IOException"> if the bind operation fails, or if the socket
		'''                     is already bound. </exception>
		''' <exception cref="SecurityException">       if a {@code SecurityManager} is present and
		''' its {@code checkListen} method doesn't allow the operation. </exception>
		''' <exception cref="IllegalArgumentException"> if endpoint is a
		'''          SocketAddress subclass not supported by this socket
		''' @since 1.4 </exception>
		Public Overridable Sub bind(  endpoint As SocketAddress,   backlog As Integer)
			If closed Then Throw New SocketException("Socket is closed")
			If (Not oldImpl) AndAlso bound Then Throw New SocketException("Already bound")
			If endpoint Is Nothing Then endpoint = New InetSocketAddress(0)
			If Not(TypeOf endpoint Is InetSocketAddress) Then Throw New IllegalArgumentException("Unsupported address type")
			Dim epoint As InetSocketAddress = CType(endpoint, InetSocketAddress)
			If epoint.unresolved Then Throw New SocketException("Unresolved address")
			If backlog < 1 Then backlog = 50
			Try
				Dim security As SecurityManager = System.securityManager
				If security IsNot Nothing Then security.checkListen(epoint.port)
				impl.bind(epoint.address, epoint.port)
				impl.listen(backlog)
				bound = True
			Catch e As SecurityException
				bound = False
				Throw e
			Catch e As java.io.IOException
				bound = False
				Throw e
			End Try
		End Sub

		''' <summary>
		''' Returns the local address of this server socket.
		''' <p>
		''' If the socket was bound prior to being <seealso cref="#close closed"/>,
		''' then this method will continue to return the local address
		''' after the socket is closed.
		''' <p>
		''' If there is a security manager set, its {@code checkConnect} method is
		''' called with the local address and {@code -1} as its arguments to see
		''' if the operation is allowed. If the operation is not allowed,
		''' the <seealso cref="InetAddress#getLoopbackAddress loopback"/> address is returned.
		''' </summary>
		''' <returns>  the address to which this socket is bound,
		'''          or the loopback address if denied by the security manager,
		'''          or {@code null} if the socket is unbound.
		''' </returns>
		''' <seealso cref= SecurityManager#checkConnect </seealso>
		Public Overridable Property inetAddress As InetAddress
			Get
				If Not bound Then Return Nothing
				Try
					Dim [in] As InetAddress = impl.inetAddress
					Dim sm As SecurityManager = System.securityManager
					If sm IsNot Nothing Then sm.checkConnect([in].hostAddress, -1)
					Return [in]
				Catch e As SecurityException
					Return InetAddress.loopbackAddress
				Catch e As SocketException
					' nothing
					' If we're bound, the impl has been created
					' so we shouldn't get here
				End Try
				Return Nothing
			End Get
		End Property

		''' <summary>
		''' Returns the port number on which this socket is listening.
		''' <p>
		''' If the socket was bound prior to being <seealso cref="#close closed"/>,
		''' then this method will continue to return the port number
		''' after the socket is closed.
		''' </summary>
		''' <returns>  the port number to which this socket is listening or
		'''          -1 if the socket is not bound yet. </returns>
		Public Overridable Property localPort As Integer
			Get
				If Not bound Then Return -1
				Try
					Return impl.localPort
				Catch e As SocketException
					' nothing
					' If we're bound, the impl has been created
					' so we shouldn't get here
				End Try
				Return -1
			End Get
		End Property

		''' <summary>
		''' Returns the address of the endpoint this socket is bound to.
		''' <p>
		''' If the socket was bound prior to being <seealso cref="#close closed"/>,
		''' then this method will continue to return the address of the endpoint
		''' after the socket is closed.
		''' <p>
		''' If there is a security manager set, its {@code checkConnect} method is
		''' called with the local address and {@code -1} as its arguments to see
		''' if the operation is allowed. If the operation is not allowed,
		''' a {@code SocketAddress} representing the
		''' <seealso cref="InetAddress#getLoopbackAddress loopback"/> address and the local
		''' port to which the socket is bound is returned.
		''' </summary>
		''' <returns> a {@code SocketAddress} representing the local endpoint of
		'''         this socket, or a {@code SocketAddress} representing the
		'''         loopback address if denied by the security manager,
		'''         or {@code null} if the socket is not bound yet.
		''' </returns>
		''' <seealso cref= #getInetAddress() </seealso>
		''' <seealso cref= #getLocalPort() </seealso>
		''' <seealso cref= #bind(SocketAddress) </seealso>
		''' <seealso cref= SecurityManager#checkConnect
		''' @since 1.4 </seealso>

		Public Overridable Property localSocketAddress As SocketAddress
			Get
				If Not bound Then Return Nothing
				Return New InetSocketAddress(inetAddress, localPort)
			End Get
		End Property

		''' <summary>
		''' Listens for a connection to be made to this socket and accepts
		''' it. The method blocks until a connection is made.
		''' 
		''' <p>A new Socket {@code s} is created and, if there
		''' is a security manager,
		''' the security manager's {@code checkAccept} method is called
		''' with {@code s.getInetAddress().getHostAddress()} and
		''' {@code s.getPort()}
		''' as its arguments to ensure the operation is allowed.
		''' This could result in a SecurityException.
		''' </summary>
		''' <exception cref="IOException">  if an I/O error occurs when waiting for a
		'''               connection. </exception>
		''' <exception cref="SecurityException">  if a security manager exists and its
		'''             {@code checkAccept} method doesn't allow the operation. </exception>
		''' <exception cref="SocketTimeoutException"> if a timeout was previously set with setSoTimeout and
		'''             the timeout has been reached. </exception>
		''' <exception cref="java.nio.channels.IllegalBlockingModeException">
		'''             if this socket has an associated channel, the channel is in
		'''             non-blocking mode, and there is no connection ready to be
		'''             accepted
		''' </exception>
		''' <returns> the new Socket </returns>
		''' <seealso cref= SecurityManager#checkAccept
		''' @revised 1.4
		''' @spec JSR-51 </seealso>
		Public Overridable Function accept() As Socket
			If closed Then Throw New SocketException("Socket is closed")
			If Not bound Then Throw New SocketException("Socket is not bound yet")
			Dim s As New Socket(CType(Nothing, SocketImpl))
			implAccept(s)
			Return s
		End Function

		''' <summary>
		''' Subclasses of ServerSocket use this method to override accept()
		''' to return their own subclass of socket.  So a FooServerSocket
		''' will typically hand this method an <i>empty</i> FooSocket.  On
		''' return from implAccept the FooSocket will be connected to a client.
		''' </summary>
		''' <param name="s"> the Socket </param>
		''' <exception cref="java.nio.channels.IllegalBlockingModeException">
		'''         if this socket has an associated channel,
		'''         and the channel is in non-blocking mode </exception>
		''' <exception cref="IOException"> if an I/O error occurs when waiting
		''' for a connection.
		''' @since   JDK1.1
		''' @revised 1.4
		''' @spec JSR-51 </exception>
		Protected Friend Sub implAccept(  s As Socket)
			Dim si As SocketImpl = Nothing
			Try
				If s.impl Is Nothing Then
				  s.implmpl()
				Else
					s.impl.reset()
				End If
				si = s.impl
				s.impl = Nothing
				si.address = New InetAddress
				si.fd = New java.io.FileDescriptor
				impl.accept(si)

				Dim security As SecurityManager = System.securityManager
				If security IsNot Nothing Then security.checkAccept(si.inetAddress.hostAddress, si.port)
			Catch e As java.io.IOException
				If si IsNot Nothing Then si.reset()
				s.impl = si
				Throw e
			Catch e As SecurityException
				If si IsNot Nothing Then si.reset()
				s.impl = si
				Throw e
			End Try
			s.impl = si
			s.postAccept()
		End Sub

		''' <summary>
		''' Closes this socket.
		''' 
		''' Any thread currently blocked in <seealso cref="#accept()"/> will throw
		''' a <seealso cref="SocketException"/>.
		''' 
		''' <p> If this socket has an associated channel then the channel is closed
		''' as well.
		''' </summary>
		''' <exception cref="IOException">  if an I/O error occurs when closing the socket.
		''' @revised 1.4
		''' @spec JSR-51 </exception>
		Public Overridable Sub close()
			SyncLock closeLock
				If closed Then Return
				If created Then impl.close()
				closed = True
			End SyncLock
		End Sub

		''' <summary>
		''' Returns the unique <seealso cref="java.nio.channels.ServerSocketChannel"/> object
		''' associated with this socket, if any.
		''' 
		''' <p> A server socket will have a channel if, and only if, the channel
		''' itself was created via the {@link
		''' java.nio.channels.ServerSocketChannel#open ServerSocketChannel.open}
		''' method.
		''' </summary>
		''' <returns>  the server-socket channel associated with this socket,
		'''          or {@code null} if this socket was not created
		'''          for a channel
		''' 
		''' @since 1.4
		''' @spec JSR-51 </returns>
		Public Overridable Property channel As java.nio.channels.ServerSocketChannel
			Get
				Return Nothing
			End Get
		End Property

		''' <summary>
		''' Returns the binding state of the ServerSocket.
		''' </summary>
		''' <returns> true if the ServerSocket successfully bound to an address
		''' @since 1.4 </returns>
		Public Overridable Property bound As Boolean
			Get
				' Before 1.3 ServerSockets were always bound during creation
				Return bound OrElse oldImpl
			End Get
		End Property

		''' <summary>
		''' Returns the closed state of the ServerSocket.
		''' </summary>
		''' <returns> true if the socket has been closed
		''' @since 1.4 </returns>
		Public Overridable Property closed As Boolean
			Get
				SyncLock closeLock
					Return closed
				End SyncLock
			End Get
		End Property

		''' <summary>
		''' Enable/disable <seealso cref="SocketOptions#SO_TIMEOUT SO_TIMEOUT"/> with the
		''' specified timeout, in milliseconds.  With this option set to a non-zero
		''' timeout, a call to accept() for this ServerSocket
		''' will block for only this amount of time.  If the timeout expires,
		''' a <B>java.net.SocketTimeoutException</B> is raised, though the
		''' ServerSocket is still valid.  The option <B>must</B> be enabled
		''' prior to entering the blocking operation to have effect.  The
		''' timeout must be {@code > 0}.
		''' A timeout of zero is interpreted as an infinite timeout. </summary>
		''' <param name="timeout"> the specified timeout, in milliseconds </param>
		''' <exception cref="SocketException"> if there is an error in
		''' the underlying protocol, such as a TCP error.
		''' @since   JDK1.1 </exception>
		''' <seealso cref= #getSoTimeout() </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Property soTimeout As Integer
			Set(  timeout As Integer)
				If closed Then Throw New SocketException("Socket is closed")
				impl.optionion(SocketOptions.SO_TIMEOUT, New Integer?(timeout))
			End Set
			Get
				If closed Then Throw New SocketException("Socket is closed")
				Dim o As Object = impl.getOption(SocketOptions.SO_TIMEOUT)
				' extra type safety 
				If TypeOf o Is Integer? Then
					Return CInt(Fix(o))
				Else
					Return 0
				End If
			End Get
		End Property


		''' <summary>
		''' Enable/disable the <seealso cref="SocketOptions#SO_REUSEADDR SO_REUSEADDR"/>
		''' socket option.
		''' <p>
		''' When a TCP connection is closed the connection may remain
		''' in a timeout state for a period of time after the connection
		''' is closed (typically known as the {@code TIME_WAIT} state
		''' or {@code 2MSL} wait state).
		''' For applications using a well known socket address or port
		''' it may not be possible to bind a socket to the required
		''' {@code SocketAddress} if there is a connection in the
		''' timeout state involving the socket address or port.
		''' <p>
		''' Enabling <seealso cref="SocketOptions#SO_REUSEADDR SO_REUSEADDR"/> prior to
		''' binding the socket using <seealso cref="#bind(SocketAddress)"/> allows the socket
		''' to be bound even though a previous connection is in a timeout state.
		''' <p>
		''' When a {@code ServerSocket} is created the initial setting
		''' of <seealso cref="SocketOptions#SO_REUSEADDR SO_REUSEADDR"/> is not defined.
		''' Applications can use <seealso cref="#getReuseAddress()"/> to determine the initial
		''' setting of <seealso cref="SocketOptions#SO_REUSEADDR SO_REUSEADDR"/>.
		''' <p>
		''' The behaviour when <seealso cref="SocketOptions#SO_REUSEADDR SO_REUSEADDR"/> is
		''' enabled or disabled after a socket is bound (See <seealso cref="#isBound()"/>)
		''' is not defined.
		''' </summary>
		''' <param name="on">  whether to enable or disable the socket option </param>
		''' <exception cref="SocketException"> if an error occurs enabling or
		'''            disabling the <seealso cref="SocketOptions#SO_REUSEADDR SO_REUSEADDR"/>
		'''            socket option, or the socket is closed.
		''' @since 1.4 </exception>
		''' <seealso cref= #getReuseAddress() </seealso>
		''' <seealso cref= #bind(SocketAddress) </seealso>
		''' <seealso cref= #isBound() </seealso>
		''' <seealso cref= #isClosed() </seealso>
		Public Overridable Property reuseAddress As Boolean
			Set(  [on] As Boolean)
				If closed Then Throw New SocketException("Socket is closed")
				impl.optionion(SocketOptions.SO_REUSEADDR, Convert.ToBoolean([on]))
			End Set
			Get
				If closed Then Throw New SocketException("Socket is closed")
				Return CBool(impl.getOption(SocketOptions.SO_REUSEADDR))
			End Get
		End Property


		''' <summary>
		''' Returns the implementation address and implementation port of
		''' this socket as a {@code String}.
		''' <p>
		''' If there is a security manager set, its {@code checkConnect} method is
		''' called with the local address and {@code -1} as its arguments to see
		''' if the operation is allowed. If the operation is not allowed,
		''' an {@code InetAddress} representing the
		''' <seealso cref="InetAddress#getLoopbackAddress loopback"/> address is returned as
		''' the implementation address.
		''' </summary>
		''' <returns>  a string representation of this socket. </returns>
		Public Overrides Function ToString() As String
			If Not bound Then Return "ServerSocket[unbound]"
			Dim [in] As InetAddress
			If System.securityManager IsNot Nothing Then
				[in] = InetAddress.loopbackAddress
			Else
				[in] = impl.inetAddress
			End If
			Return "ServerSocket[addr=" & [in] & ",localport=" & impl.localPort & "]"
		End Function

		Friend Overridable Sub setBound()
			bound = True
		End Sub

		Friend Overridable Sub setCreated()
			created = True
		End Sub

		''' <summary>
		''' The factory for all server sockets.
		''' </summary>
		Private Shared factory As SocketImplFactory = Nothing

		''' <summary>
		''' Sets the server socket implementation factory for the
		''' application. The factory can be specified only once.
		''' <p>
		''' When an application creates a new server socket, the socket
		''' implementation factory's {@code createSocketImpl} method is
		''' called to create the actual socket implementation.
		''' <p>
		''' Passing {@code null} to the method is a no-op unless the factory
		''' was already set.
		''' <p>
		''' If there is a security manager, this method first calls
		''' the security manager's {@code checkSetFactory} method
		''' to ensure the operation is allowed.
		''' This could result in a SecurityException.
		''' </summary>
		''' <param name="fac">   the desired factory. </param>
		''' <exception cref="IOException">  if an I/O error occurs when setting the
		'''               socket factory. </exception>
		''' <exception cref="SocketException">  if the factory has already been defined. </exception>
		''' <exception cref="SecurityException">  if a security manager exists and its
		'''             {@code checkSetFactory} method doesn't allow the operation. </exception>
		''' <seealso cref=        java.net.SocketImplFactory#createSocketImpl() </seealso>
		''' <seealso cref=        SecurityManager#checkSetFactory </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Shared Property socketFactory As SocketImplFactory
			Set(  fac As SocketImplFactory)
				If factory IsNot Nothing Then Throw New SocketException("factory already defined")
				Dim security As SecurityManager = System.securityManager
				If security IsNot Nothing Then security.checkSetFactory()
				factory = fac
			End Set
		End Property

		''' <summary>
		''' Sets a default proposed value for the
		''' <seealso cref="SocketOptions#SO_RCVBUF SO_RCVBUF"/> option for sockets
		''' accepted from this {@code ServerSocket}. The value actually set
		''' in the accepted socket must be determined by calling
		''' <seealso cref="Socket#getReceiveBufferSize()"/> after the socket
		''' is returned by <seealso cref="#accept()"/>.
		''' <p>
		''' The value of <seealso cref="SocketOptions#SO_RCVBUF SO_RCVBUF"/> is used both to
		''' set the size of the internal socket receive buffer, and to set the size
		''' of the TCP receive window that is advertized to the remote peer.
		''' <p>
		''' It is possible to change the value subsequently, by calling
		''' <seealso cref="Socket#setReceiveBufferSize(int)"/>. However, if the application
		''' wishes to allow a receive window larger than 64K bytes, as defined by RFC1323
		''' then the proposed value must be set in the ServerSocket <B>before</B>
		''' it is bound to a local address. This implies, that the ServerSocket must be
		''' created with the no-argument constructor, then setReceiveBufferSize() must
		''' be called and lastly the ServerSocket is bound to an address by calling bind().
		''' <p>
		''' Failure to do this will not cause an error, and the buffer size may be set to the
		''' requested value but the TCP receive window in sockets accepted from
		''' this ServerSocket will be no larger than 64K bytes.
		''' </summary>
		''' <exception cref="SocketException"> if there is an error
		''' in the underlying protocol, such as a TCP error.
		''' </exception>
		''' <param name="size"> the size to which to set the receive buffer
		''' size. This value must be greater than 0.
		''' </param>
		''' <exception cref="IllegalArgumentException"> if the
		''' value is 0 or is negative.
		''' 
		''' @since 1.4 </exception>
		''' <seealso cref= #getReceiveBufferSize </seealso>
		 <MethodImpl(MethodImplOptions.Synchronized)> _
		 Public Overridable Property receiveBufferSize As Integer
			 Set(  size As Integer)
				If Not(size > 0) Then Throw New IllegalArgumentException("negative receive size")
				If closed Then Throw New SocketException("Socket is closed")
				impl.optionion(SocketOptions.SO_RCVBUF, New Integer?(size))
			 End Set
			 Get
				If closed Then Throw New SocketException("Socket is closed")
				Dim result As Integer = 0
				Dim o As Object = impl.getOption(SocketOptions.SO_RCVBUF)
				If TypeOf o Is Integer? Then result = CInt(Fix(o))
				Return result
			End Get
		 End Property


		''' <summary>
		''' Sets performance preferences for this ServerSocket.
		''' 
		''' <p> Sockets use the TCP/IP protocol by default.  Some implementations
		''' may offer alternative protocols which have different performance
		''' characteristics than TCP/IP.  This method allows the application to
		''' express its own preferences as to how these tradeoffs should be made
		''' when the implementation chooses from the available protocols.
		''' 
		''' <p> Performance preferences are described by three integers
		''' whose values indicate the relative importance of short connection time,
		''' low latency, and high bandwidth.  The absolute values of the integers
		''' are irrelevant; in order to choose a protocol the values are simply
		''' compared, with larger values indicating stronger preferences.  If the
		''' application prefers short connection time over both low latency and high
		''' bandwidth, for example, then it could invoke this method with the values
		''' {@code (1, 0, 0)}.  If the application prefers high bandwidth above low
		''' latency, and low latency above short connection time, then it could
		''' invoke this method with the values {@code (0, 1, 2)}.
		''' 
		''' <p> Invoking this method after this socket has been bound
		''' will have no effect. This implies that in order to use this capability
		''' requires the socket to be created with the no-argument constructor.
		''' </summary>
		''' <param name="connectionTime">
		'''         An {@code int} expressing the relative importance of a short
		'''         connection time
		''' </param>
		''' <param name="latency">
		'''         An {@code int} expressing the relative importance of low
		'''         latency
		''' </param>
		''' <param name="bandwidth">
		'''         An {@code int} expressing the relative importance of high
		'''         bandwidth
		''' 
		''' @since 1.5 </param>
		Public Overridable Sub setPerformancePreferences(  connectionTime As Integer,   latency As Integer,   bandwidth As Integer)
			' Not implemented yet 
		End Sub

	End Class

End Namespace