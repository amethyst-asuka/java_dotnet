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
	''' This class implements client sockets (also called just
	''' "sockets"). A socket is an endpoint for communication
	''' between two machines.
	''' <p>
	''' The actual work of the socket is performed by an instance of the
	''' {@code SocketImpl} class. An application, by changing
	''' the socket factory that creates the socket implementation,
	''' can configure itself to create sockets appropriate to the local
	''' firewall.
	''' 
	''' @author  unascribed </summary>
	''' <seealso cref=     java.net.Socket#setSocketImplFactory(java.net.SocketImplFactory) </seealso>
	''' <seealso cref=     java.net.SocketImpl </seealso>
	''' <seealso cref=     java.nio.channels.SocketChannel
	''' @since   JDK1.0 </seealso>
	Public Class Socket
		Implements java.io.Closeable

		''' <summary>
		''' Various states of this socket.
		''' </summary>
		Private created As Boolean = False
		Private bound As Boolean = False
		Private connected As Boolean = False
		Private closed As Boolean = False
		Private closeLock As New Object
		Private shutIn As Boolean = False
		Private shutOut As Boolean = False

		''' <summary>
		''' The implementation of this Socket.
		''' </summary>
		Friend impl As SocketImpl

		''' <summary>
		''' Are we using an older SocketImpl?
		''' </summary>
		Private oldImpl As Boolean = False

		''' <summary>
		''' Creates an unconnected socket, with the
		''' system-default type of SocketImpl.
		''' 
		''' @since   JDK1.1
		''' @revised 1.4
		''' </summary>
		Public Sub New()
			implmpl()
		End Sub

		''' <summary>
		''' Creates an unconnected socket, specifying the type of proxy, if any,
		''' that should be used regardless of any other settings.
		''' <P>
		''' If there is a security manager, its {@code checkConnect} method
		''' is called with the proxy host address and port number
		''' as its arguments. This could result in a SecurityException.
		''' <P>
		''' Examples:
		''' <UL> <LI>{@code Socket s = new Socket(Proxy.NO_PROXY);} will create
		''' a plain socket ignoring any other proxy configuration.</LI>
		''' <LI>{@code Socket s = new Socket(new Proxy(Proxy.Type.SOCKS, new InetSocketAddress("socks.mydom.com", 1080)));}
		''' will create a socket connecting through the specified SOCKS proxy
		''' server.</LI>
		''' </UL>
		''' </summary>
		''' <param name="proxy"> a <seealso cref="java.net.Proxy Proxy"/> object specifying what kind
		'''              of proxying should be used. </param>
		''' <exception cref="IllegalArgumentException"> if the proxy is of an invalid type
		'''          or {@code null}. </exception>
		''' <exception cref="SecurityException"> if a security manager is present and
		'''                           permission to connect to the proxy is
		'''                           denied. </exception>
		''' <seealso cref= java.net.ProxySelector </seealso>
		''' <seealso cref= java.net.Proxy
		''' 
		''' @since   1.5 </seealso>
		Public Sub New(ByVal proxy_Renamed As Proxy)
			' Create a copy of Proxy as a security measure
			If proxy_Renamed Is Nothing Then Throw New IllegalArgumentException("Invalid Proxy")
			Dim p As Proxy = If(proxy_Renamed Is Proxy.NO_PROXY, Proxy.NO_PROXY, sun.net.ApplicationProxy.create(proxy_Renamed))
			Dim type As Proxy.Type = p.type()
			If type = Proxy.Type.SOCKS OrElse type = Proxy.Type.HTTP Then
				Dim security As SecurityManager = System.securityManager
				Dim epoint As InetSocketAddress = CType(p.address(), InetSocketAddress)
				If epoint.address IsNot Nothing Then checkAddress(epoint.address, "Socket")
				If security IsNot Nothing Then
					If epoint.unresolved Then epoint = New InetSocketAddress(epoint.hostName, epoint.port)
					If epoint.unresolved Then
						security.checkConnect(epoint.hostName, epoint.port)
					Else
						security.checkConnect(epoint.address.hostAddress, epoint.port)
					End If
				End If
				impl = If(type = Proxy.Type.SOCKS, New SocksSocketImpl(p), New HttpConnectSocketImpl(p))
				impl.socket = Me
			Else
				If p Is Proxy.NO_PROXY Then
					If factory Is Nothing Then
						impl = New PlainSocketImpl
						impl.socket = Me
					Else
						implmpl()
					End If
				Else
					Throw New IllegalArgumentException("Invalid Proxy")
				End If
			End If
		End Sub

		''' <summary>
		''' Creates an unconnected Socket with a user-specified
		''' SocketImpl.
		''' <P> </summary>
		''' <param name="impl"> an instance of a <B>SocketImpl</B>
		''' the subclass wishes to use on the Socket.
		''' </param>
		''' <exception cref="SocketException"> if there is an error in the underlying protocol,
		''' such as a TCP error.
		''' @since   JDK1.1 </exception>
		Protected Friend Sub New(ByVal impl As SocketImpl)
			Me.impl = impl
			If impl IsNot Nothing Then
				checkOldImpl()
				Me.impl.socket = Me
			End If
		End Sub

		''' <summary>
		''' Creates a stream socket and connects it to the specified port
		''' number on the named host.
		''' <p>
		''' If the specified host is {@code null} it is the equivalent of
		''' specifying the address as
		''' <seealso cref="java.net.InetAddress#getByName InetAddress.getByName"/>{@code (null)}.
		''' In other words, it is equivalent to specifying an address of the
		''' loopback interface. </p>
		''' <p>
		''' If the application has specified a server socket factory, that
		''' factory's {@code createSocketImpl} method is called to create
		''' the actual socket implementation. Otherwise a "plain" socket is created.
		''' <p>
		''' If there is a security manager, its
		''' {@code checkConnect} method is called
		''' with the host address and {@code port}
		''' as its arguments. This could result in a SecurityException.
		''' </summary>
		''' <param name="host">   the host name, or {@code null} for the loopback address. </param>
		''' <param name="port">   the port number.
		''' </param>
		''' <exception cref="UnknownHostException"> if the IP address of
		''' the host could not be determined.
		''' </exception>
		''' <exception cref="IOException">  if an I/O error occurs when creating the socket. </exception>
		''' <exception cref="SecurityException">  if a security manager exists and its
		'''             {@code checkConnect} method doesn't allow the operation. </exception>
		''' <exception cref="IllegalArgumentException"> if the port parameter is outside
		'''             the specified range of valid port values, which is between
		'''             0 and 65535, inclusive. </exception>
		''' <seealso cref=        java.net.Socket#setSocketImplFactory(java.net.SocketImplFactory) </seealso>
		''' <seealso cref=        java.net.SocketImpl </seealso>
		''' <seealso cref=        java.net.SocketImplFactory#createSocketImpl() </seealso>
		''' <seealso cref=        SecurityManager#checkConnect </seealso>
		Public Sub New(ByVal host As String, ByVal port As Integer)
			Me.New(If(host IsNot Nothing, New InetSocketAddress(host, port), New InetSocketAddress(InetAddress.getByName(Nothing), port)), CType(Nothing, SocketAddress), True)
		End Sub

		''' <summary>
		''' Creates a stream socket and connects it to the specified port
		''' number at the specified IP address.
		''' <p>
		''' If the application has specified a socket factory, that factory's
		''' {@code createSocketImpl} method is called to create the
		''' actual socket implementation. Otherwise a "plain" socket is created.
		''' <p>
		''' If there is a security manager, its
		''' {@code checkConnect} method is called
		''' with the host address and {@code port}
		''' as its arguments. This could result in a SecurityException.
		''' </summary>
		''' <param name="address">   the IP address. </param>
		''' <param name="port">      the port number. </param>
		''' <exception cref="IOException">  if an I/O error occurs when creating the socket. </exception>
		''' <exception cref="SecurityException">  if a security manager exists and its
		'''             {@code checkConnect} method doesn't allow the operation. </exception>
		''' <exception cref="IllegalArgumentException"> if the port parameter is outside
		'''             the specified range of valid port values, which is between
		'''             0 and 65535, inclusive. </exception>
		''' <exception cref="NullPointerException"> if {@code address} is null. </exception>
		''' <seealso cref=        java.net.Socket#setSocketImplFactory(java.net.SocketImplFactory) </seealso>
		''' <seealso cref=        java.net.SocketImpl </seealso>
		''' <seealso cref=        java.net.SocketImplFactory#createSocketImpl() </seealso>
		''' <seealso cref=        SecurityManager#checkConnect </seealso>
		Public Sub New(ByVal address As InetAddress, ByVal port As Integer)
			Me.New(If(address IsNot Nothing, New InetSocketAddress(address, port), Nothing), CType(Nothing, SocketAddress), True)
		End Sub

		''' <summary>
		''' Creates a socket and connects it to the specified remote host on
		''' the specified remote port. The Socket will also bind() to the local
		''' address and port supplied.
		''' <p>
		''' If the specified host is {@code null} it is the equivalent of
		''' specifying the address as
		''' <seealso cref="java.net.InetAddress#getByName InetAddress.getByName"/>{@code (null)}.
		''' In other words, it is equivalent to specifying an address of the
		''' loopback interface. </p>
		''' <p>
		''' A local port number of {@code zero} will let the system pick up a
		''' free port in the {@code bind} operation.</p>
		''' <p>
		''' If there is a security manager, its
		''' {@code checkConnect} method is called
		''' with the host address and {@code port}
		''' as its arguments. This could result in a SecurityException.
		''' </summary>
		''' <param name="host"> the name of the remote host, or {@code null} for the loopback address. </param>
		''' <param name="port"> the remote port </param>
		''' <param name="localAddr"> the local address the socket is bound to, or
		'''        {@code null} for the {@code anyLocal} address. </param>
		''' <param name="localPort"> the local port the socket is bound to, or
		'''        {@code zero} for a system selected free port. </param>
		''' <exception cref="IOException">  if an I/O error occurs when creating the socket. </exception>
		''' <exception cref="SecurityException">  if a security manager exists and its
		'''             {@code checkConnect} method doesn't allow the connection
		'''             to the destination, or if its {@code checkListen} method
		'''             doesn't allow the bind to the local port. </exception>
		''' <exception cref="IllegalArgumentException"> if the port parameter or localPort
		'''             parameter is outside the specified range of valid port values,
		'''             which is between 0 and 65535, inclusive. </exception>
		''' <seealso cref=        SecurityManager#checkConnect
		''' @since   JDK1.1 </seealso>
		Public Sub New(ByVal host As String, ByVal port As Integer, ByVal localAddr As InetAddress, ByVal localPort As Integer)
			Me.New(If(host IsNot Nothing, New InetSocketAddress(host, port), New InetSocketAddress(InetAddress.getByName(Nothing), port)), New InetSocketAddress(localAddr, localPort), True)
		End Sub

		''' <summary>
		''' Creates a socket and connects it to the specified remote address on
		''' the specified remote port. The Socket will also bind() to the local
		''' address and port supplied.
		''' <p>
		''' If the specified local address is {@code null} it is the equivalent of
		''' specifying the address as the AnyLocal address
		''' (see <seealso cref="java.net.InetAddress#isAnyLocalAddress InetAddress.isAnyLocalAddress"/>{@code ()}).
		''' <p>
		''' A local port number of {@code zero} will let the system pick up a
		''' free port in the {@code bind} operation.</p>
		''' <p>
		''' If there is a security manager, its
		''' {@code checkConnect} method is called
		''' with the host address and {@code port}
		''' as its arguments. This could result in a SecurityException.
		''' </summary>
		''' <param name="address"> the remote address </param>
		''' <param name="port"> the remote port </param>
		''' <param name="localAddr"> the local address the socket is bound to, or
		'''        {@code null} for the {@code anyLocal} address. </param>
		''' <param name="localPort"> the local port the socket is bound to or
		'''        {@code zero} for a system selected free port. </param>
		''' <exception cref="IOException">  if an I/O error occurs when creating the socket. </exception>
		''' <exception cref="SecurityException">  if a security manager exists and its
		'''             {@code checkConnect} method doesn't allow the connection
		'''             to the destination, or if its {@code checkListen} method
		'''             doesn't allow the bind to the local port. </exception>
		''' <exception cref="IllegalArgumentException"> if the port parameter or localPort
		'''             parameter is outside the specified range of valid port values,
		'''             which is between 0 and 65535, inclusive. </exception>
		''' <exception cref="NullPointerException"> if {@code address} is null. </exception>
		''' <seealso cref=        SecurityManager#checkConnect
		''' @since   JDK1.1 </seealso>
		Public Sub New(ByVal address As InetAddress, ByVal port As Integer, ByVal localAddr As InetAddress, ByVal localPort As Integer)
			Me.New(If(address IsNot Nothing, New InetSocketAddress(address, port), Nothing), New InetSocketAddress(localAddr, localPort), True)
		End Sub

		''' <summary>
		''' Creates a stream socket and connects it to the specified port
		''' number on the named host.
		''' <p>
		''' If the specified host is {@code null} it is the equivalent of
		''' specifying the address as
		''' <seealso cref="java.net.InetAddress#getByName InetAddress.getByName"/>{@code (null)}.
		''' In other words, it is equivalent to specifying an address of the
		''' loopback interface. </p>
		''' <p>
		''' If the stream argument is {@code true}, this creates a
		''' stream socket. If the stream argument is {@code false}, it
		''' creates a datagram socket.
		''' <p>
		''' If the application has specified a server socket factory, that
		''' factory's {@code createSocketImpl} method is called to create
		''' the actual socket implementation. Otherwise a "plain" socket is created.
		''' <p>
		''' If there is a security manager, its
		''' {@code checkConnect} method is called
		''' with the host address and {@code port}
		''' as its arguments. This could result in a SecurityException.
		''' <p>
		''' If a UDP socket is used, TCP/IP related socket options will not apply.
		''' </summary>
		''' <param name="host">     the host name, or {@code null} for the loopback address. </param>
		''' <param name="port">     the port number. </param>
		''' <param name="stream">   a {@code boolean} indicating whether this is
		'''                      a stream socket or a datagram socket. </param>
		''' <exception cref="IOException">  if an I/O error occurs when creating the socket. </exception>
		''' <exception cref="SecurityException">  if a security manager exists and its
		'''             {@code checkConnect} method doesn't allow the operation. </exception>
		''' <exception cref="IllegalArgumentException"> if the port parameter is outside
		'''             the specified range of valid port values, which is between
		'''             0 and 65535, inclusive. </exception>
		''' <seealso cref=        java.net.Socket#setSocketImplFactory(java.net.SocketImplFactory) </seealso>
		''' <seealso cref=        java.net.SocketImpl </seealso>
		''' <seealso cref=        java.net.SocketImplFactory#createSocketImpl() </seealso>
		''' <seealso cref=        SecurityManager#checkConnect </seealso>
		''' @deprecated Use DatagramSocket instead for UDP transport. 
		<Obsolete("Use DatagramSocket instead for UDP transport.")> _
		Public Sub New(ByVal host As String, ByVal port As Integer, ByVal stream As Boolean)
			Me.New(If(host IsNot Nothing, New InetSocketAddress(host, port), New InetSocketAddress(InetAddress.getByName(Nothing), port)), CType(Nothing, SocketAddress), stream)
		End Sub

		''' <summary>
		''' Creates a socket and connects it to the specified port number at
		''' the specified IP address.
		''' <p>
		''' If the stream argument is {@code true}, this creates a
		''' stream socket. If the stream argument is {@code false}, it
		''' creates a datagram socket.
		''' <p>
		''' If the application has specified a server socket factory, that
		''' factory's {@code createSocketImpl} method is called to create
		''' the actual socket implementation. Otherwise a "plain" socket is created.
		''' 
		''' <p>If there is a security manager, its
		''' {@code checkConnect} method is called
		''' with {@code host.getHostAddress()} and {@code port}
		''' as its arguments. This could result in a SecurityException.
		''' <p>
		''' If UDP socket is used, TCP/IP related socket options will not apply.
		''' </summary>
		''' <param name="host">     the IP address. </param>
		''' <param name="port">      the port number. </param>
		''' <param name="stream">    if {@code true}, create a stream socket;
		'''                       otherwise, create a datagram socket. </param>
		''' <exception cref="IOException">  if an I/O error occurs when creating the socket. </exception>
		''' <exception cref="SecurityException">  if a security manager exists and its
		'''             {@code checkConnect} method doesn't allow the operation. </exception>
		''' <exception cref="IllegalArgumentException"> if the port parameter is outside
		'''             the specified range of valid port values, which is between
		'''             0 and 65535, inclusive. </exception>
		''' <exception cref="NullPointerException"> if {@code host} is null. </exception>
		''' <seealso cref=        java.net.Socket#setSocketImplFactory(java.net.SocketImplFactory) </seealso>
		''' <seealso cref=        java.net.SocketImpl </seealso>
		''' <seealso cref=        java.net.SocketImplFactory#createSocketImpl() </seealso>
		''' <seealso cref=        SecurityManager#checkConnect </seealso>
		''' @deprecated Use DatagramSocket instead for UDP transport. 
		<Obsolete("Use DatagramSocket instead for UDP transport.")> _
		Public Sub New(ByVal host As InetAddress, ByVal port As Integer, ByVal stream As Boolean)
			Me.New(If(host IsNot Nothing, New InetSocketAddress(host, port), Nothing), New InetSocketAddress(0), stream)
		End Sub

		Private Sub New(ByVal address As SocketAddress, ByVal localAddr As SocketAddress, ByVal stream As Boolean)
			implmpl()

			' backward compatibility
			If address Is Nothing Then Throw New NullPointerException

			Try
				createImpl(stream)
				If localAddr IsNot Nothing Then bind(localAddr)
				connect(address)
'JAVA TO VB CONVERTER TODO TASK: There is no equivalent in VB to Java 'multi-catch' syntax:
			Catch java.io.IOException Or IllegalArgumentException Or SecurityException e
				Try
					close()
				Catch ce As java.io.IOException
					e.addSuppressed(ce)
				End Try
				Throw e
			End Try
		End Sub

		''' <summary>
		''' Creates the socket implementation.
		''' </summary>
		''' <param name="stream"> a {@code boolean} value : {@code true} for a TCP socket,
		'''               {@code false} for UDP. </param>
		''' <exception cref="IOException"> if creation fails
		''' @since 1.4 </exception>
		 Friend Overridable Sub createImpl(ByVal stream As Boolean)
			If impl Is Nothing Then implmpl()
			Try
				impl.create(stream)
				created = True
			Catch e As java.io.IOException
				Throw New SocketException(e.Message)
			End Try
		 End Sub

		Private Sub checkOldImpl()
			If impl Is Nothing Then Return
			' SocketImpl.connect() is a protected method, therefore we need to use
			' getDeclaredMethod, therefore we need permission to access the member

			oldImpl = java.security.AccessController.doPrivileged(New PrivilegedActionAnonymousInnerClassHelper(Of T)
		End Sub

		Private Class PrivilegedActionAnonymousInnerClassHelper(Of T)
			Implements java.security.PrivilegedAction(Of T)

			Public Overridable Function run() As Boolean?
				Dim clazz As Class = outerInstance.impl.GetType()
				Do
					Try
						clazz.getDeclaredMethod("connect", GetType(SocketAddress), GetType(Integer))
						Return Boolean.FALSE
					Catch e As NoSuchMethodException
						clazz = clazz.BaseType
						' java.net.SocketImpl class will always have this abstract method.
						' If we have not found it by now in the hierarchy then it does not
						' exist, we are an old style impl.
						If clazz.Equals(GetType(java.net.SocketImpl)) Then Return Boolean.TRUE
					End Try
				Loop
			End Function
		End Class

		''' <summary>
		''' Sets impl to the system-default type of SocketImpl.
		''' @since 1.4
		''' </summary>
		Friend Overridable Sub setImpl()
			If factory IsNot Nothing Then
				impl = factory.createSocketImpl()
				checkOldImpl()
			Else
				' No need to do a checkOldImpl() here, we know it's an up to date
				' SocketImpl!
				impl = New SocksSocketImpl
			End If
			If impl IsNot Nothing Then impl.socket = Me
		End Sub


		''' <summary>
		''' Get the {@code SocketImpl} attached to this socket, creating
		''' it if necessary.
		''' </summary>
		''' <returns>  the {@code SocketImpl} attached to that ServerSocket. </returns>
		''' <exception cref="SocketException"> if creation fails
		''' @since 1.4 </exception>
		Friend Overridable Property impl As SocketImpl
			Get
				If Not created Then createImpl(True)
				Return impl
			End Get
		End Property

		''' <summary>
		''' Connects this socket to the server.
		''' </summary>
		''' <param name="endpoint"> the {@code SocketAddress} </param>
		''' <exception cref="IOException"> if an error occurs during the connection </exception>
		''' <exception cref="java.nio.channels.IllegalBlockingModeException">
		'''          if this socket has an associated channel,
		'''          and the channel is in non-blocking mode </exception>
		''' <exception cref="IllegalArgumentException"> if endpoint is null or is a
		'''          SocketAddress subclass not supported by this socket
		''' @since 1.4
		''' @spec JSR-51 </exception>
		Public Overridable Sub connect(ByVal endpoint As SocketAddress)
			connect(endpoint, 0)
		End Sub

		''' <summary>
		''' Connects this socket to the server with a specified timeout value.
		''' A timeout of zero is interpreted as an infinite timeout. The connection
		''' will then block until established or an error occurs.
		''' </summary>
		''' <param name="endpoint"> the {@code SocketAddress} </param>
		''' <param name="timeout">  the timeout value to be used in milliseconds. </param>
		''' <exception cref="IOException"> if an error occurs during the connection </exception>
		''' <exception cref="SocketTimeoutException"> if timeout expires before connecting </exception>
		''' <exception cref="java.nio.channels.IllegalBlockingModeException">
		'''          if this socket has an associated channel,
		'''          and the channel is in non-blocking mode </exception>
		''' <exception cref="IllegalArgumentException"> if endpoint is null or is a
		'''          SocketAddress subclass not supported by this socket
		''' @since 1.4
		''' @spec JSR-51 </exception>
		Public Overridable Sub connect(ByVal endpoint As SocketAddress, ByVal timeout As Integer)
			If endpoint Is Nothing Then Throw New IllegalArgumentException("connect: The address can't be null")

			If timeout < 0 Then Throw New IllegalArgumentException("connect: timeout can't be negative")

			If closed Then Throw New SocketException("Socket is closed")

			If (Not oldImpl) AndAlso connected Then Throw New SocketException("already connected")

			If Not(TypeOf endpoint Is InetSocketAddress) Then Throw New IllegalArgumentException("Unsupported address type")

			Dim epoint As InetSocketAddress = CType(endpoint, InetSocketAddress)
			Dim addr As InetAddress = epoint.address
			Dim port_Renamed As Integer = epoint.port
			checkAddress(addr, "connect")

			Dim security As SecurityManager = System.securityManager
			If security IsNot Nothing Then
				If epoint.unresolved Then
					security.checkConnect(epoint.hostName, port_Renamed)
				Else
					security.checkConnect(addr.hostAddress, port_Renamed)
				End If
			End If
			If Not created Then createImpl(True)
			If Not oldImpl Then
				impl.connect(epoint, timeout)
			ElseIf timeout = 0 Then
				If epoint.unresolved Then
					impl.connect(addr.hostName, port_Renamed)
				Else
					impl.connect(addr, port_Renamed)
				End If
			Else
				Throw New UnsupportedOperationException("SocketImpl.connect(addr, timeout)")
			End If
			connected = True
	'        
	'         * If the socket was not bound before the connect, it is now because
	'         * the kernel will have picked an ephemeral port & a local address
	'         
			bound = True
		End Sub

		''' <summary>
		''' Binds the socket to a local address.
		''' <P>
		''' If the address is {@code null}, then the system will pick up
		''' an ephemeral port and a valid local address to bind the socket.
		''' </summary>
		''' <param name="bindpoint"> the {@code SocketAddress} to bind to </param>
		''' <exception cref="IOException"> if the bind operation fails, or if the socket
		'''                     is already bound. </exception>
		''' <exception cref="IllegalArgumentException"> if bindpoint is a
		'''          SocketAddress subclass not supported by this socket </exception>
		''' <exception cref="SecurityException">  if a security manager exists and its
		'''          {@code checkListen} method doesn't allow the bind
		'''          to the local port.
		''' 
		''' @since   1.4 </exception>
		''' <seealso cref= #isBound </seealso>
		Public Overridable Sub bind(ByVal bindpoint As SocketAddress)
			If closed Then Throw New SocketException("Socket is closed")
			If (Not oldImpl) AndAlso bound Then Throw New SocketException("Already bound")

			If bindpoint IsNot Nothing AndAlso (Not(TypeOf bindpoint Is InetSocketAddress)) Then Throw New IllegalArgumentException("Unsupported address type")
			Dim epoint As InetSocketAddress = CType(bindpoint, InetSocketAddress)
			If epoint IsNot Nothing AndAlso epoint.unresolved Then Throw New SocketException("Unresolved address")
			If epoint Is Nothing Then epoint = New InetSocketAddress(0)
			Dim addr As InetAddress = epoint.address
			Dim port_Renamed As Integer = epoint.port
			checkAddress(addr, "bind")
			Dim security As SecurityManager = System.securityManager
			If security IsNot Nothing Then security.checkListen(port_Renamed)
			impl.bind(addr, port_Renamed)
			bound = True
		End Sub

		Private Sub checkAddress(ByVal addr As InetAddress, ByVal op As String)
			If addr Is Nothing Then Return
			If Not(TypeOf addr Is Inet4Address OrElse TypeOf addr Is Inet6Address) Then Throw New IllegalArgumentException(op & ": invalid address type")
		End Sub

		''' <summary>
		''' set the flags after an accept() call.
		''' </summary>
		Friend Sub postAccept()
			connected = True
			created = True
			bound = True
		End Sub

		Friend Overridable Sub setCreated()
			created = True
		End Sub

		Friend Overridable Sub setBound()
			bound = True
		End Sub

		Friend Overridable Sub setConnected()
			connected = True
		End Sub

		''' <summary>
		''' Returns the address to which the socket is connected.
		''' <p>
		''' If the socket was connected prior to being <seealso cref="#close closed"/>,
		''' then this method will continue to return the connected address
		''' after the socket is closed.
		''' </summary>
		''' <returns>  the remote IP address to which this socket is connected,
		'''          or {@code null} if the socket is not connected. </returns>
		Public Overridable Property inetAddress As InetAddress
			Get
				If Not connected Then Return Nothing
				Try
					Return impl.inetAddress
				Catch e As SocketException
				End Try
				Return Nothing
			End Get
		End Property

		''' <summary>
		''' Gets the local address to which the socket is bound.
		''' <p>
		''' If there is a security manager set, its {@code checkConnect} method is
		''' called with the local address and {@code -1} as its arguments to see
		''' if the operation is allowed. If the operation is not allowed,
		''' the <seealso cref="InetAddress#getLoopbackAddress loopback"/> address is returned.
		''' </summary>
		''' <returns> the local address to which the socket is bound,
		'''         the loopback address if denied by the security manager, or
		'''         the wildcard address if the socket is closed or not bound yet.
		''' @since   JDK1.1
		''' </returns>
		''' <seealso cref= SecurityManager#checkConnect </seealso>
		Public Overridable Property localAddress As InetAddress
			Get
				' This is for backward compatibility
				If Not bound Then Return InetAddress.anyLocalAddress()
				Dim [in] As InetAddress = Nothing
				Try
					[in] = CType(impl.getOption(SocketOptions.SO_BINDADDR), InetAddress)
					Dim sm As SecurityManager = System.securityManager
					If sm IsNot Nothing Then sm.checkConnect([in].hostAddress, -1)
					If [in].anyLocalAddress Then [in] = InetAddress.anyLocalAddress()
				Catch e As SecurityException
					[in] = InetAddress.loopbackAddress
				Catch e As Exception
					[in] = InetAddress.anyLocalAddress() ' "0.0.0.0"
				End Try
				Return [in]
			End Get
		End Property

		''' <summary>
		''' Returns the remote port number to which this socket is connected.
		''' <p>
		''' If the socket was connected prior to being <seealso cref="#close closed"/>,
		''' then this method will continue to return the connected port number
		''' after the socket is closed.
		''' </summary>
		''' <returns>  the remote port number to which this socket is connected, or
		'''          0 if the socket is not connected yet. </returns>
		Public Overridable Property port As Integer
			Get
				If Not connected Then Return 0
				Try
					Return impl.port
				Catch e As SocketException
					' Shouldn't happen as we're connected
				End Try
				Return -1
			End Get
		End Property

		''' <summary>
		''' Returns the local port number to which this socket is bound.
		''' <p>
		''' If the socket was bound prior to being <seealso cref="#close closed"/>,
		''' then this method will continue to return the local port number
		''' after the socket is closed.
		''' </summary>
		''' <returns>  the local port number to which this socket is bound or -1
		'''          if the socket is not bound yet. </returns>
		Public Overridable Property localPort As Integer
			Get
				If Not bound Then Return -1
				Try
					Return impl.localPort
				Catch e As SocketException
					' shouldn't happen as we're bound
				End Try
				Return -1
			End Get
		End Property

		''' <summary>
		''' Returns the address of the endpoint this socket is connected to, or
		''' {@code null} if it is unconnected.
		''' <p>
		''' If the socket was connected prior to being <seealso cref="#close closed"/>,
		''' then this method will continue to return the connected address
		''' after the socket is closed.
		''' 
		''' </summary>
		''' <returns> a {@code SocketAddress} representing the remote endpoint of this
		'''         socket, or {@code null} if it is not connected yet. </returns>
		''' <seealso cref= #getInetAddress() </seealso>
		''' <seealso cref= #getPort() </seealso>
		''' <seealso cref= #connect(SocketAddress, int) </seealso>
		''' <seealso cref= #connect(SocketAddress)
		''' @since 1.4 </seealso>
		Public Overridable Property remoteSocketAddress As SocketAddress
			Get
				If Not connected Then Return Nothing
				Return New InetSocketAddress(inetAddress, port)
			End Get
		End Property

		''' <summary>
		''' Returns the address of the endpoint this socket is bound to.
		''' <p>
		''' If a socket bound to an endpoint represented by an
		''' {@code InetSocketAddress } is <seealso cref="#close closed"/>,
		''' then this method will continue to return an {@code InetSocketAddress}
		''' after the socket is closed. In that case the returned
		''' {@code InetSocketAddress}'s address is the
		''' <seealso cref="InetAddress#isAnyLocalAddress wildcard"/> address
		''' and its port is the local port that it was bound to.
		''' <p>
		''' If there is a security manager set, its {@code checkConnect} method is
		''' called with the local address and {@code -1} as its arguments to see
		''' if the operation is allowed. If the operation is not allowed,
		''' a {@code SocketAddress} representing the
		''' <seealso cref="InetAddress#getLoopbackAddress loopback"/> address and the local
		''' port to which this socket is bound is returned.
		''' </summary>
		''' <returns> a {@code SocketAddress} representing the local endpoint of
		'''         this socket, or a {@code SocketAddress} representing the
		'''         loopback address if denied by the security manager, or
		'''         {@code null} if the socket is not bound yet.
		''' </returns>
		''' <seealso cref= #getLocalAddress() </seealso>
		''' <seealso cref= #getLocalPort() </seealso>
		''' <seealso cref= #bind(SocketAddress) </seealso>
		''' <seealso cref= SecurityManager#checkConnect
		''' @since 1.4 </seealso>

		Public Overridable Property localSocketAddress As SocketAddress
			Get
				If Not bound Then Return Nothing
				Return New InetSocketAddress(localAddress, localPort)
			End Get
		End Property

		''' <summary>
		''' Returns the unique <seealso cref="java.nio.channels.SocketChannel SocketChannel"/>
		''' object associated with this socket, if any.
		''' 
		''' <p> A socket will have a channel if, and only if, the channel itself was
		''' created via the {@link java.nio.channels.SocketChannel#open
		''' SocketChannel.open} or {@link
		''' java.nio.channels.ServerSocketChannel#accept ServerSocketChannel.accept}
		''' methods.
		''' </summary>
		''' <returns>  the socket channel associated with this socket,
		'''          or {@code null} if this socket was not created
		'''          for a channel
		''' 
		''' @since 1.4
		''' @spec JSR-51 </returns>
		Public Overridable Property channel As java.nio.channels.SocketChannel
			Get
				Return Nothing
			End Get
		End Property

		''' <summary>
		''' Returns an input stream for this socket.
		''' 
		''' <p> If this socket has an associated channel then the resulting input
		''' stream delegates all of its operations to the channel.  If the channel
		''' is in non-blocking mode then the input stream's {@code read} operations
		''' will throw an <seealso cref="java.nio.channels.IllegalBlockingModeException"/>.
		''' 
		''' <p>Under abnormal conditions the underlying connection may be
		''' broken by the remote host or the network software (for example
		''' a connection reset in the case of TCP connections). When a
		''' broken connection is detected by the network software the
		''' following applies to the returned input stream :-
		''' 
		''' <ul>
		''' 
		'''   <li><p>The network software may discard bytes that are buffered
		'''   by the socket. Bytes that aren't discarded by the network
		'''   software can be read using <seealso cref="java.io.InputStream#read read"/>.
		''' 
		'''   <li><p>If there are no bytes buffered on the socket, or all
		'''   buffered bytes have been consumed by
		'''   <seealso cref="java.io.InputStream#read read"/>, then all subsequent
		'''   calls to <seealso cref="java.io.InputStream#read read"/> will throw an
		'''   <seealso cref="java.io.IOException IOException"/>.
		''' 
		'''   <li><p>If there are no bytes buffered on the socket, and the
		'''   socket has not been closed using <seealso cref="#close close"/>, then
		'''   <seealso cref="java.io.InputStream#available available"/> will
		'''   return {@code 0}.
		''' 
		''' </ul>
		''' 
		''' <p> Closing the returned <seealso cref="java.io.InputStream InputStream"/>
		''' will close the associated socket.
		''' </summary>
		''' <returns>     an input stream for reading bytes from this socket. </returns>
		''' <exception cref="IOException">  if an I/O error occurs when creating the
		'''             input stream, the socket is closed, the socket is
		'''             not connected, or the socket input has been shutdown
		'''             using <seealso cref="#shutdownInput()"/>
		''' 
		''' @revised 1.4
		''' @spec JSR-51 </exception>
		Public Overridable Property inputStream As java.io.InputStream
			Get
				If closed Then Throw New SocketException("Socket is closed")
				If Not connected Then Throw New SocketException("Socket is not connected")
				If inputShutdown Then Throw New SocketException("Socket input is shutdown")
				Dim s As Socket = Me
				Dim [is] As java.io.InputStream = Nothing
				Try
					[is] = java.security.AccessController.doPrivileged(New PrivilegedExceptionActionAnonymousInnerClassHelper(Of T)
				Catch e As java.security.PrivilegedActionException
					Throw CType(e.exception, java.io.IOException)
				End Try
				Return [is]
			End Get
		End Property

		Private Class PrivilegedExceptionActionAnonymousInnerClassHelper(Of T)
			Implements java.security.PrivilegedExceptionAction(Of T)

			Public Overridable Function run() As java.io.InputStream
				Return outerInstance.impl.inputStream
			End Function
		End Class

		''' <summary>
		''' Returns an output stream for this socket.
		''' 
		''' <p> If this socket has an associated channel then the resulting output
		''' stream delegates all of its operations to the channel.  If the channel
		''' is in non-blocking mode then the output stream's {@code write}
		''' operations will throw an {@link
		''' java.nio.channels.IllegalBlockingModeException}.
		''' 
		''' <p> Closing the returned <seealso cref="java.io.OutputStream OutputStream"/>
		''' will close the associated socket.
		''' </summary>
		''' <returns>     an output stream for writing bytes to this socket. </returns>
		''' <exception cref="IOException">  if an I/O error occurs when creating the
		'''               output stream or if the socket is not connected.
		''' @revised 1.4
		''' @spec JSR-51 </exception>
		Public Overridable Property outputStream As java.io.OutputStream
			Get
				If closed Then Throw New SocketException("Socket is closed")
				If Not connected Then Throw New SocketException("Socket is not connected")
				If outputShutdown Then Throw New SocketException("Socket output is shutdown")
				Dim s As Socket = Me
				Dim os As java.io.OutputStream = Nothing
				Try
					os = java.security.AccessController.doPrivileged(New PrivilegedExceptionActionAnonymousInnerClassHelper2(Of T)
				Catch e As java.security.PrivilegedActionException
					Throw CType(e.exception, java.io.IOException)
				End Try
				Return os
			End Get
		End Property

		Private Class PrivilegedExceptionActionAnonymousInnerClassHelper2(Of T)
			Implements java.security.PrivilegedExceptionAction(Of T)

			Public Overridable Function run() As java.io.OutputStream
				Return outerInstance.impl.outputStream
			End Function
		End Class

		''' <summary>
		''' Enable/disable <seealso cref="SocketOptions#TCP_NODELAY TCP_NODELAY"/>
		''' (disable/enable Nagle's algorithm).
		''' </summary>
		''' <param name="on"> {@code true} to enable TCP_NODELAY,
		''' {@code false} to disable.
		''' </param>
		''' <exception cref="SocketException"> if there is an error
		''' in the underlying protocol, such as a TCP error.
		''' 
		''' @since   JDK1.1
		''' </exception>
		''' <seealso cref= #getTcpNoDelay() </seealso>
		Public Overridable Property tcpNoDelay As Boolean
			Set(ByVal [on] As Boolean)
				If closed Then Throw New SocketException("Socket is closed")
				impl.optionion(SocketOptions.TCP_NODELAY, Convert.ToBoolean([on]))
			End Set
			Get
				If closed Then Throw New SocketException("Socket is closed")
				Return CBool(impl.getOption(SocketOptions.TCP_NODELAY))
			End Get
		End Property


		''' <summary>
		''' Enable/disable <seealso cref="SocketOptions#SO_LINGER SO_LINGER"/> with the
		''' specified linger time in seconds. The maximum timeout value is platform
		''' specific.
		''' 
		''' The setting only affects socket close.
		''' </summary>
		''' <param name="on">     whether or not to linger on. </param>
		''' <param name="linger"> how long to linger for, if on is true. </param>
		''' <exception cref="SocketException"> if there is an error
		''' in the underlying protocol, such as a TCP error. </exception>
		''' <exception cref="IllegalArgumentException"> if the linger value is negative.
		''' @since JDK1.1 </exception>
		''' <seealso cref= #getSoLinger() </seealso>
		Public Overridable Sub setSoLinger(ByVal [on] As Boolean, ByVal linger As Integer)
			If closed Then Throw New SocketException("Socket is closed")
			If Not [on] Then
				impl.optionion(SocketOptions.SO_LINGER, New Boolean?([on]))
			Else
				If linger < 0 Then Throw New IllegalArgumentException("invalid value for SO_LINGER")
				If linger > 65535 Then linger = 65535
				impl.optionion(SocketOptions.SO_LINGER, New Integer?(linger))
			End If
		End Sub

		''' <summary>
		''' Returns setting for <seealso cref="SocketOptions#SO_LINGER SO_LINGER"/>.
		''' -1 returns implies that the
		''' option is disabled.
		''' 
		''' The setting only affects socket close.
		''' </summary>
		''' <returns> the setting for <seealso cref="SocketOptions#SO_LINGER SO_LINGER"/>. </returns>
		''' <exception cref="SocketException"> if there is an error
		''' in the underlying protocol, such as a TCP error.
		''' @since   JDK1.1 </exception>
		''' <seealso cref= #setSoLinger(boolean, int) </seealso>
		Public Overridable Property soLinger As Integer
			Get
				If closed Then Throw New SocketException("Socket is closed")
				Dim o As Object = impl.getOption(SocketOptions.SO_LINGER)
				If TypeOf o Is Integer? Then
					Return CInt(Fix(o))
				Else
					Return -1
				End If
			End Get
		End Property

		''' <summary>
		''' Send one byte of urgent data on the socket. The byte to be sent is the lowest eight
		''' bits of the data parameter. The urgent byte is
		''' sent after any preceding writes to the socket OutputStream
		''' and before any future writes to the OutputStream. </summary>
		''' <param name="data"> The byte of data to send </param>
		''' <exception cref="IOException"> if there is an error
		'''  sending the data.
		''' @since 1.4 </exception>
		Public Overridable Sub sendUrgentData(ByVal data As Integer)
			If Not impl.supportsUrgentData() Then Throw New SocketException("Urgent data not supported")
			impl.sendUrgentData(data)
		End Sub

		''' <summary>
		''' Enable/disable <seealso cref="SocketOptions#SO_OOBINLINE SO_OOBINLINE"/>
		''' (receipt of TCP urgent data)
		''' 
		''' By default, this option is disabled and TCP urgent data received on a
		''' socket is silently discarded. If the user wishes to receive urgent data, then
		''' this option must be enabled. When enabled, urgent data is received
		''' inline with normal data.
		''' <p>
		''' Note, only limited support is provided for handling incoming urgent
		''' data. In particular, no notification of incoming urgent data is provided
		''' and there is no capability to distinguish between normal data and urgent
		''' data unless provided by a higher level protocol.
		''' </summary>
		''' <param name="on"> {@code true} to enable
		'''           <seealso cref="SocketOptions#SO_OOBINLINE SO_OOBINLINE"/>,
		'''           {@code false} to disable.
		''' </param>
		''' <exception cref="SocketException"> if there is an error
		''' in the underlying protocol, such as a TCP error.
		''' 
		''' @since   1.4
		''' </exception>
		''' <seealso cref= #getOOBInline() </seealso>
		Public Overridable Property oOBInline As Boolean
			Set(ByVal [on] As Boolean)
				If closed Then Throw New SocketException("Socket is closed")
				impl.optionion(SocketOptions.SO_OOBINLINE, Convert.ToBoolean([on]))
			End Set
			Get
				If closed Then Throw New SocketException("Socket is closed")
				Return CBool(impl.getOption(SocketOptions.SO_OOBINLINE))
			End Get
		End Property


		''' <summary>
		'''  Enable/disable <seealso cref="SocketOptions#SO_TIMEOUT SO_TIMEOUT"/>
		'''  with the specified timeout, in milliseconds. With this option set
		'''  to a non-zero timeout, a read() call on the InputStream associated with
		'''  this Socket will block for only this amount of time.  If the timeout
		'''  expires, a <B>java.net.SocketTimeoutException</B> is raised, though the
		'''  Socket is still valid. The option <B>must</B> be enabled
		'''  prior to entering the blocking operation to have effect. The
		'''  timeout must be {@code > 0}.
		'''  A timeout of zero is interpreted as an infinite timeout.
		''' </summary>
		''' <param name="timeout"> the specified timeout, in milliseconds. </param>
		''' <exception cref="SocketException"> if there is an error
		''' in the underlying protocol, such as a TCP error.
		''' @since   JDK 1.1 </exception>
		''' <seealso cref= #getSoTimeout() </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Property soTimeout As Integer
			Set(ByVal timeout As Integer)
				If closed Then Throw New SocketException("Socket is closed")
				If timeout < 0 Then Throw New IllegalArgumentException("timeout can't be negative")
    
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
		''' Sets the <seealso cref="SocketOptions#SO_SNDBUF SO_SNDBUF"/> option to the
		''' specified value for this {@code Socket}.
		''' The <seealso cref="SocketOptions#SO_SNDBUF SO_SNDBUF"/> option is used by the
		''' platform's networking code as a hint for the size to set the underlying
		''' network I/O buffers.
		''' 
		''' <p>Because <seealso cref="SocketOptions#SO_SNDBUF SO_SNDBUF"/> is a hint,
		''' applications that want to verify what size the buffers were set to
		''' should call <seealso cref="#getSendBufferSize()"/>.
		''' </summary>
		''' <exception cref="SocketException"> if there is an error
		''' in the underlying protocol, such as a TCP error.
		''' </exception>
		''' <param name="size"> the size to which to set the send buffer
		''' size. This value must be greater than 0.
		''' </param>
		''' <exception cref="IllegalArgumentException"> if the
		''' value is 0 or is negative.
		''' </exception>
		''' <seealso cref= #getSendBufferSize()
		''' @since 1.2 </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Property sendBufferSize As Integer
			Set(ByVal size As Integer)
				If Not(size > 0) Then Throw New IllegalArgumentException("negative send size")
				If closed Then Throw New SocketException("Socket is closed")
				impl.optionion(SocketOptions.SO_SNDBUF, New Integer?(size))
			End Set
			Get
				If closed Then Throw New SocketException("Socket is closed")
				Dim result As Integer = 0
				Dim o As Object = impl.getOption(SocketOptions.SO_SNDBUF)
				If TypeOf o Is Integer? Then result = CInt(Fix(o))
				Return result
			End Get
		End Property


		''' <summary>
		''' Sets the <seealso cref="SocketOptions#SO_RCVBUF SO_RCVBUF"/> option to the
		''' specified value for this {@code Socket}. The
		''' <seealso cref="SocketOptions#SO_RCVBUF SO_RCVBUF"/> option is
		''' used by the platform's networking code as a hint for the size to set
		''' the underlying network I/O buffers.
		''' 
		''' <p>Increasing the receive buffer size can increase the performance of
		''' network I/O for high-volume connection, while decreasing it can
		''' help reduce the backlog of incoming data.
		''' 
		''' <p>Because <seealso cref="SocketOptions#SO_RCVBUF SO_RCVBUF"/> is a hint,
		''' applications that want to verify what size the buffers were set to
		''' should call <seealso cref="#getReceiveBufferSize()"/>.
		''' 
		''' <p>The value of <seealso cref="SocketOptions#SO_RCVBUF SO_RCVBUF"/> is also used
		''' to set the TCP receive window that is advertized to the remote peer.
		''' Generally, the window size can be modified at any time when a socket is
		''' connected. However, if a receive window larger than 64K is required then
		''' this must be requested <B>before</B> the socket is connected to the
		''' remote peer. There are two cases to be aware of:
		''' <ol>
		''' <li>For sockets accepted from a ServerSocket, this must be done by calling
		''' <seealso cref="ServerSocket#setReceiveBufferSize(int)"/> before the ServerSocket
		''' is bound to a local address.<p></li>
		''' <li>For client sockets, setReceiveBufferSize() must be called before
		''' connecting the socket to its remote peer.</li></ol> </summary>
		''' <param name="size"> the size to which to set the receive buffer
		''' size. This value must be greater than 0.
		''' </param>
		''' <exception cref="IllegalArgumentException"> if the value is 0 or is
		''' negative.
		''' </exception>
		''' <exception cref="SocketException"> if there is an error
		''' in the underlying protocol, such as a TCP error.
		''' </exception>
		''' <seealso cref= #getReceiveBufferSize() </seealso>
		''' <seealso cref= ServerSocket#setReceiveBufferSize(int)
		''' @since 1.2 </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Property receiveBufferSize As Integer
			Set(ByVal size As Integer)
				If size <= 0 Then Throw New IllegalArgumentException("invalid receive size")
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
		''' Enable/disable <seealso cref="SocketOptions#SO_KEEPALIVE SO_KEEPALIVE"/>.
		''' </summary>
		''' <param name="on">  whether or not to have socket keep alive turned on. </param>
		''' <exception cref="SocketException"> if there is an error
		''' in the underlying protocol, such as a TCP error.
		''' @since 1.3 </exception>
		''' <seealso cref= #getKeepAlive() </seealso>
		Public Overridable Property keepAlive As Boolean
			Set(ByVal [on] As Boolean)
				If closed Then Throw New SocketException("Socket is closed")
				impl.optionion(SocketOptions.SO_KEEPALIVE, Convert.ToBoolean([on]))
			End Set
			Get
				If closed Then Throw New SocketException("Socket is closed")
				Return CBool(impl.getOption(SocketOptions.SO_KEEPALIVE))
			End Get
		End Property


		''' <summary>
		''' Sets traffic class or type-of-service octet in the IP
		''' header for packets sent from this Socket.
		''' As the underlying network implementation may ignore this
		''' value applications should consider it a hint.
		''' 
		''' <P> The tc <B>must</B> be in the range {@code 0 <= tc <=
		''' 255} or an IllegalArgumentException will be thrown.
		''' <p>Notes:
		''' <p>For Internet Protocol v4 the value consists of an
		''' {@code integer}, the least significant 8 bits of which
		''' represent the value of the TOS octet in IP packets sent by
		''' the socket.
		''' RFC 1349 defines the TOS values as follows:
		''' 
		''' <UL>
		''' <LI><CODE>IPTOS_LOWCOST (0x02)</CODE></LI>
		''' <LI><CODE>IPTOS_RELIABILITY (0x04)</CODE></LI>
		''' <LI><CODE>IPTOS_THROUGHPUT (0x08)</CODE></LI>
		''' <LI><CODE>IPTOS_LOWDELAY (0x10)</CODE></LI>
		''' </UL>
		''' The last low order bit is always ignored as this
		''' corresponds to the MBZ (must be zero) bit.
		''' <p>
		''' Setting bits in the precedence field may result in a
		''' SocketException indicating that the operation is not
		''' permitted.
		''' <p>
		''' As RFC 1122 section 4.2.4.2 indicates, a compliant TCP
		''' implementation should, but is not required to, let application
		''' change the TOS field during the lifetime of a connection.
		''' So whether the type-of-service field can be changed after the
		''' TCP connection has been established depends on the implementation
		''' in the underlying platform. Applications should not assume that
		''' they can change the TOS field after the connection.
		''' <p>
		''' For Internet Protocol v6 {@code tc} is the value that
		''' would be placed into the sin6_flowinfo field of the IP header.
		''' </summary>
		''' <param name="tc">        an {@code int} value for the bitset. </param>
		''' <exception cref="SocketException"> if there is an error setting the
		''' traffic class or type-of-service
		''' @since 1.4 </exception>
		''' <seealso cref= #getTrafficClass </seealso>
		''' <seealso cref= SocketOptions#IP_TOS </seealso>
		Public Overridable Property trafficClass As Integer
			Set(ByVal tc As Integer)
				If tc < 0 OrElse tc > 255 Then Throw New IllegalArgumentException("tc is not in range 0 -- 255")
    
				If closed Then Throw New SocketException("Socket is closed")
				Try
					impl.optionion(SocketOptions.IP_TOS, tc)
				Catch se As SocketException
					' not supported if socket already connected
					' Solaris returns error in such cases
					If Not connected Then Throw se
				End Try
			End Set
			Get
				Return CInt(Fix(impl.getOption(SocketOptions.IP_TOS)))
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
		''' Enabling <seealso cref="SocketOptions#SO_REUSEADDR SO_REUSEADDR"/>
		''' prior to binding the socket using <seealso cref="#bind(SocketAddress)"/> allows
		''' the socket to be bound even though a previous connection is in a timeout
		''' state.
		''' <p>
		''' When a {@code Socket} is created the initial setting
		''' of <seealso cref="SocketOptions#SO_REUSEADDR SO_REUSEADDR"/> is disabled.
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
		''' <seealso cref= #isClosed() </seealso>
		''' <seealso cref= #isBound() </seealso>
		Public Overridable Property reuseAddress As Boolean
			Set(ByVal [on] As Boolean)
				If closed Then Throw New SocketException("Socket is closed")
				impl.optionion(SocketOptions.SO_REUSEADDR, Convert.ToBoolean([on]))
			End Set
			Get
				If closed Then Throw New SocketException("Socket is closed")
				Return CBool(impl.getOption(SocketOptions.SO_REUSEADDR))
			End Get
		End Property


		''' <summary>
		''' Closes this socket.
		''' <p>
		''' Any thread currently blocked in an I/O operation upon this socket
		''' will throw a <seealso cref="SocketException"/>.
		''' <p>
		''' Once a socket has been closed, it is not available for further networking
		''' use (i.e. can't be reconnected or rebound). A new socket needs to be
		''' created.
		''' 
		''' <p> Closing this socket will also close the socket's
		''' <seealso cref="java.io.InputStream InputStream"/> and
		''' <seealso cref="java.io.OutputStream OutputStream"/>.
		''' 
		''' <p> If this socket has an associated channel then the channel is closed
		''' as well.
		''' </summary>
		''' <exception cref="IOException">  if an I/O error occurs when closing this socket.
		''' @revised 1.4
		''' @spec JSR-51 </exception>
		''' <seealso cref= #isClosed </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub close()
			SyncLock closeLock
				If closed Then Return
				If created Then impl.close()
				closed = True
			End SyncLock
		End Sub

		''' <summary>
		''' Places the input stream for this socket at "end of stream".
		''' Any data sent to the input stream side of the socket is acknowledged
		''' and then silently discarded.
		''' <p>
		''' If you read from a socket input stream after invoking this method on the
		''' socket, the stream's {@code available} method will return 0, and its
		''' {@code read} methods will return {@code -1} (end of stream).
		''' </summary>
		''' <exception cref="IOException"> if an I/O error occurs when shutting down this
		''' socket.
		''' 
		''' @since 1.3 </exception>
		''' <seealso cref= java.net.Socket#shutdownOutput() </seealso>
		''' <seealso cref= java.net.Socket#close() </seealso>
		''' <seealso cref= java.net.Socket#setSoLinger(boolean, int) </seealso>
		''' <seealso cref= #isInputShutdown </seealso>
		Public Overridable Sub shutdownInput()
			If closed Then Throw New SocketException("Socket is closed")
			If Not connected Then Throw New SocketException("Socket is not connected")
			If inputShutdown Then Throw New SocketException("Socket input is already shutdown")
			impl.shutdownInput()
			shutIn = True
		End Sub

		''' <summary>
		''' Disables the output stream for this socket.
		''' For a TCP socket, any previously written data will be sent
		''' followed by TCP's normal connection termination sequence.
		''' 
		''' If you write to a socket output stream after invoking
		''' shutdownOutput() on the socket, the stream will throw
		''' an IOException.
		''' </summary>
		''' <exception cref="IOException"> if an I/O error occurs when shutting down this
		''' socket.
		''' 
		''' @since 1.3 </exception>
		''' <seealso cref= java.net.Socket#shutdownInput() </seealso>
		''' <seealso cref= java.net.Socket#close() </seealso>
		''' <seealso cref= java.net.Socket#setSoLinger(boolean, int) </seealso>
		''' <seealso cref= #isOutputShutdown </seealso>
		Public Overridable Sub shutdownOutput()
			If closed Then Throw New SocketException("Socket is closed")
			If Not connected Then Throw New SocketException("Socket is not connected")
			If outputShutdown Then Throw New SocketException("Socket output is already shutdown")
			impl.shutdownOutput()
			shutOut = True
		End Sub

		''' <summary>
		''' Converts this socket to a {@code String}.
		''' </summary>
		''' <returns>  a string representation of this socket. </returns>
		Public Overrides Function ToString() As String
			Try
				If connected Then Return "Socket[addr=" & impl.inetAddress & ",port=" & impl.port & ",localport=" & impl.localPort & "]"
			Catch e As SocketException
			End Try
			Return "Socket[unconnected]"
		End Function

		''' <summary>
		''' Returns the connection state of the socket.
		''' <p>
		''' Note: Closing a socket doesn't clear its connection state, which means
		''' this method will return {@code true} for a closed socket
		''' (see <seealso cref="#isClosed()"/>) if it was successfuly connected prior
		''' to being closed.
		''' </summary>
		''' <returns> true if the socket was successfuly connected to a server
		''' @since 1.4 </returns>
		Public Overridable Property connected As Boolean
			Get
				' Before 1.3 Sockets were always connected during creation
				Return connected OrElse oldImpl
			End Get
		End Property

		''' <summary>
		''' Returns the binding state of the socket.
		''' <p>
		''' Note: Closing a socket doesn't clear its binding state, which means
		''' this method will return {@code true} for a closed socket
		''' (see <seealso cref="#isClosed()"/>) if it was successfuly bound prior
		''' to being closed.
		''' </summary>
		''' <returns> true if the socket was successfuly bound to an address
		''' @since 1.4 </returns>
		''' <seealso cref= #bind </seealso>
		Public Overridable Property bound As Boolean
			Get
				' Before 1.3 Sockets were always bound during creation
				Return bound OrElse oldImpl
			End Get
		End Property

		''' <summary>
		''' Returns the closed state of the socket.
		''' </summary>
		''' <returns> true if the socket has been closed
		''' @since 1.4 </returns>
		''' <seealso cref= #close </seealso>
		Public Overridable Property closed As Boolean
			Get
				SyncLock closeLock
					Return closed
				End SyncLock
			End Get
		End Property

		''' <summary>
		''' Returns whether the read-half of the socket connection is closed.
		''' </summary>
		''' <returns> true if the input of the socket has been shutdown
		''' @since 1.4 </returns>
		''' <seealso cref= #shutdownInput </seealso>
		Public Overridable Property inputShutdown As Boolean
			Get
				Return shutIn
			End Get
		End Property

		''' <summary>
		''' Returns whether the write-half of the socket connection is closed.
		''' </summary>
		''' <returns> true if the output of the socket has been shutdown
		''' @since 1.4 </returns>
		''' <seealso cref= #shutdownOutput </seealso>
		Public Overridable Property outputShutdown As Boolean
			Get
				Return shutOut
			End Get
		End Property

		''' <summary>
		''' The factory for all client sockets.
		''' </summary>
		Private Shared factory As SocketImplFactory = Nothing

		''' <summary>
		''' Sets the client socket implementation factory for the
		''' application. The factory can be specified only once.
		''' <p>
		''' When an application creates a new client socket, the socket
		''' implementation factory's {@code createSocketImpl} method is
		''' called to create the actual socket implementation.
		''' <p>
		''' Passing {@code null} to the method is a no-op unless the factory
		''' was already set.
		''' <p>If there is a security manager, this method first calls
		''' the security manager's {@code checkSetFactory} method
		''' to ensure the operation is allowed.
		''' This could result in a SecurityException.
		''' </summary>
		''' <param name="fac">   the desired factory. </param>
		''' <exception cref="IOException">  if an I/O error occurs when setting the
		'''               socket factory. </exception>
		''' <exception cref="SocketException">  if the factory is already defined. </exception>
		''' <exception cref="SecurityException">  if a security manager exists and its
		'''             {@code checkSetFactory} method doesn't allow the operation. </exception>
		''' <seealso cref=        java.net.SocketImplFactory#createSocketImpl() </seealso>
		''' <seealso cref=        SecurityManager#checkSetFactory </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Shared Property socketImplFactory As SocketImplFactory
			Set(ByVal fac As SocketImplFactory)
				If factory IsNot Nothing Then Throw New SocketException("factory already defined")
				Dim security As SecurityManager = System.securityManager
				If security IsNot Nothing Then security.checkSetFactory()
				factory = fac
			End Set
		End Property

		''' <summary>
		''' Sets performance preferences for this socket.
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
		''' compared, with larger values indicating stronger preferences. Negative
		''' values represent a lower priority than positive values. If the
		''' application prefers short connection time over both low latency and high
		''' bandwidth, for example, then it could invoke this method with the values
		''' {@code (1, 0, 0)}.  If the application prefers high bandwidth above low
		''' latency, and low latency above short connection time, then it could
		''' invoke this method with the values {@code (0, 1, 2)}.
		''' 
		''' <p> Invoking this method after this socket has been connected
		''' will have no effect.
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
		Public Overridable Sub setPerformancePreferences(ByVal connectionTime As Integer, ByVal latency As Integer, ByVal bandwidth As Integer)
			' Not implemented yet 
		End Sub
	End Class

End Namespace