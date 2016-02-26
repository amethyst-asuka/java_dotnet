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
	''' This class represents a socket for sending and receiving datagram packets.
	''' 
	''' <p>A datagram socket is the sending or receiving point for a packet
	''' delivery service. Each packet sent or received on a datagram socket
	''' is individually addressed and routed. Multiple packets sent from
	''' one machine to another may be routed differently, and may arrive in
	''' any order.
	''' 
	''' <p> Where possible, a newly constructed {@code DatagramSocket} has the
	''' <seealso cref="SocketOptions#SO_BROADCAST SO_BROADCAST"/> socket option enabled so as
	''' to allow the transmission of broadcast datagrams. In order to receive
	''' broadcast packets a DatagramSocket should be bound to the wildcard address.
	''' In some implementations, broadcast packets may also be received when
	''' a DatagramSocket is bound to a more specific address.
	''' <p>
	''' Example:
	''' {@code
	'''              DatagramSocket s = new DatagramSocket(null);
	'''              s.bind(new InetSocketAddress(8888));
	''' }
	''' Which is equivalent to:
	''' {@code
	'''              DatagramSocket s = new DatagramSocket(8888);
	''' }
	''' Both cases will create a DatagramSocket able to receive broadcasts on
	''' UDP port 8888.
	''' 
	''' @author  Pavani Diwanji </summary>
	''' <seealso cref=     java.net.DatagramPacket </seealso>
	''' <seealso cref=     java.nio.channels.DatagramChannel
	''' @since JDK1.0 </seealso>
	Public Class DatagramSocket
		Implements java.io.Closeable

		''' <summary>
		''' Various states of this socket.
		''' </summary>
		Private created As Boolean = False
		Private bound As Boolean = False
		Private closed As Boolean = False
		Private closeLock As New Object

	'    
	'     * The implementation of this DatagramSocket.
	'     
		Friend impl As DatagramSocketImpl

		''' <summary>
		''' Are we using an older DatagramSocketImpl?
		''' </summary>
		Friend oldImpl As Boolean = False

		''' <summary>
		''' Set when a socket is ST_CONNECTED until we are certain
		''' that any packets which might have been received prior
		''' to calling connect() but not read by the application
		''' have been read. During this time we check the source
		''' address of all packets received to be sure they are from
		''' the connected destination. Other packets are read but
		''' silently dropped.
		''' </summary>
		Private explicitFilter As Boolean = False
		Private bytesLeftToFilter As Integer
	'    
	'     * Connection state:
	'     * ST_NOT_CONNECTED = socket not connected
	'     * ST_CONNECTED = socket connected
	'     * ST_CONNECTED_NO_IMPL = socket connected but not at impl level
	'     
		Friend Const ST_NOT_CONNECTED As Integer = 0
		Friend Const ST_CONNECTED As Integer = 1
		Friend Const ST_CONNECTED_NO_IMPL As Integer = 2

		Friend connectState As Integer = ST_NOT_CONNECTED

	'    
	'     * Connected address & port
	'     
		Friend connectedAddress As InetAddress = Nothing
		Friend connectedPort As Integer = -1

		''' <summary>
		''' Connects this socket to a remote socket address (IP address + port number).
		''' Binds socket if not already bound.
		''' <p> </summary>
		''' <param name="address"> The remote address. </param>
		''' <param name="port">    The remote port </param>
		''' <exception cref="SocketException"> if binding the socket fails. </exception>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Private Sub connectInternal(ByVal address As InetAddress, ByVal port As Integer)
			If port < 0 OrElse port > &HFFFF Then Throw New IllegalArgumentException("connect: " & port)
			If address Is Nothing Then Throw New IllegalArgumentException("connect: null address")
			checkAddress(address, "connect")
			If closed Then Return
			Dim security As SecurityManager = System.securityManager
			If security IsNot Nothing Then
				If address.multicastAddress Then
					security.checkMulticast(address)
				Else
					security.checkConnect(address.hostAddress, port)
					security.checkAccept(address.hostAddress, port)
				End If
			End If

			If Not bound Then bind(New InetSocketAddress(0))

			' old impls do not support connect/disconnect
			If oldImpl OrElse (TypeOf impl Is AbstractPlainDatagramSocketImpl AndAlso CType(impl, AbstractPlainDatagramSocketImpl).nativeConnectDisabled()) Then
				connectState = ST_CONNECTED_NO_IMPL
			Else
				Try
					impl.connect(address, port)

					' socket is now connected by the impl
					connectState = ST_CONNECTED
					' Do we need to filter some packets?
					Dim avail As Integer = impl.dataAvailable()
					If avail = -1 Then Throw New SocketException
					explicitFilter = avail > 0
					If explicitFilter Then bytesLeftToFilter = receiveBufferSize
				Catch se As SocketException

					' connection will be emulated by DatagramSocket
					connectState = ST_CONNECTED_NO_IMPL
				End Try
			End If

			connectedAddress = address
			connectedPort = port
		End Sub


		''' <summary>
		''' Constructs a datagram socket and binds it to any available port
		''' on the local host machine.  The socket will be bound to the
		''' <seealso cref="InetAddress#isAnyLocalAddress wildcard"/> address,
		''' an IP address chosen by the kernel.
		''' 
		''' <p>If there is a security manager,
		''' its {@code checkListen} method is first called
		''' with 0 as its argument to ensure the operation is allowed.
		''' This could result in a SecurityException.
		''' </summary>
		''' <exception cref="SocketException">  if the socket could not be opened,
		'''               or the socket could not bind to the specified local port. </exception>
		''' <exception cref="SecurityException">  if a security manager exists and its
		'''             {@code checkListen} method doesn't allow the operation.
		''' </exception>
		''' <seealso cref= SecurityManager#checkListen </seealso>
		Public Sub New()
			Me.New(New InetSocketAddress(0))
		End Sub

		''' <summary>
		''' Creates an unbound datagram socket with the specified
		''' DatagramSocketImpl.
		''' </summary>
		''' <param name="impl"> an instance of a <B>DatagramSocketImpl</B>
		'''        the subclass wishes to use on the DatagramSocket.
		''' @since   1.4 </param>
		Protected Friend Sub New(ByVal impl As DatagramSocketImpl)
			If impl Is Nothing Then Throw New NullPointerException
			Me.impl = impl
			checkOldImpl()
		End Sub

		''' <summary>
		''' Creates a datagram socket, bound to the specified local
		''' socket address.
		''' <p>
		''' If, if the address is {@code null}, creates an unbound socket.
		''' 
		''' <p>If there is a security manager,
		''' its {@code checkListen} method is first called
		''' with the port from the socket address
		''' as its argument to ensure the operation is allowed.
		''' This could result in a SecurityException.
		''' </summary>
		''' <param name="bindaddr"> local socket address to bind, or {@code null}
		'''                 for an unbound socket.
		''' </param>
		''' <exception cref="SocketException">  if the socket could not be opened,
		'''               or the socket could not bind to the specified local port. </exception>
		''' <exception cref="SecurityException">  if a security manager exists and its
		'''             {@code checkListen} method doesn't allow the operation.
		''' </exception>
		''' <seealso cref= SecurityManager#checkListen
		''' @since   1.4 </seealso>
		Public Sub New(ByVal bindaddr As SocketAddress)
			' create a datagram socket.
			createImpl()
			If bindaddr IsNot Nothing Then
				Try
					bind(bindaddr)
				Finally
					If Not bound Then close()
				End Try
			End If
		End Sub

		''' <summary>
		''' Constructs a datagram socket and binds it to the specified port
		''' on the local host machine.  The socket will be bound to the
		''' <seealso cref="InetAddress#isAnyLocalAddress wildcard"/> address,
		''' an IP address chosen by the kernel.
		''' 
		''' <p>If there is a security manager,
		''' its {@code checkListen} method is first called
		''' with the {@code port} argument
		''' as its argument to ensure the operation is allowed.
		''' This could result in a SecurityException.
		''' </summary>
		''' <param name="port"> port to use. </param>
		''' <exception cref="SocketException">  if the socket could not be opened,
		'''               or the socket could not bind to the specified local port. </exception>
		''' <exception cref="SecurityException">  if a security manager exists and its
		'''             {@code checkListen} method doesn't allow the operation.
		''' </exception>
		''' <seealso cref= SecurityManager#checkListen </seealso>
		Public Sub New(ByVal port As Integer)
			Me.New(port, Nothing)
		End Sub

		''' <summary>
		''' Creates a datagram socket, bound to the specified local
		''' address.  The local port must be between 0 and 65535 inclusive.
		''' If the IP address is 0.0.0.0, the socket will be bound to the
		''' <seealso cref="InetAddress#isAnyLocalAddress wildcard"/> address,
		''' an IP address chosen by the kernel.
		''' 
		''' <p>If there is a security manager,
		''' its {@code checkListen} method is first called
		''' with the {@code port} argument
		''' as its argument to ensure the operation is allowed.
		''' This could result in a SecurityException.
		''' </summary>
		''' <param name="port"> local port to use </param>
		''' <param name="laddr"> local address to bind
		''' </param>
		''' <exception cref="SocketException">  if the socket could not be opened,
		'''               or the socket could not bind to the specified local port. </exception>
		''' <exception cref="SecurityException">  if a security manager exists and its
		'''             {@code checkListen} method doesn't allow the operation.
		''' </exception>
		''' <seealso cref= SecurityManager#checkListen
		''' @since   JDK1.1 </seealso>
		Public Sub New(ByVal port As Integer, ByVal laddr As InetAddress)
			Me.New(New InetSocketAddress(laddr, port))
		End Sub

		Private Sub checkOldImpl()
			If impl Is Nothing Then Return
			' DatagramSocketImpl.peekdata() is a protected method, therefore we need to use
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
				Dim cl As  [Class]() = New [Class](0){}
				cl(0) = GetType(DatagramPacket)
				outerInstance.impl.GetType().getDeclaredMethod("peekData", cl)
				Return Nothing
			End Function
		End Class

		Friend Shared implClass As  [Class] = Nothing

		Friend Overridable Sub createImpl()
			If impl Is Nothing Then
				If factory IsNot Nothing Then
					impl = factory.createDatagramSocketImpl()
					checkOldImpl()
				Else
					Dim isMulticast As Boolean = If(TypeOf Me Is MulticastSocket, True, False)
					impl = DefaultDatagramSocketImplFactory.createDatagramSocketImpl(isMulticast)

					checkOldImpl()
				End If
			End If
			' creates a udp socket
			impl.create()
			impl.datagramSocket = Me
			created = True
		End Sub

		''' <summary>
		''' Get the {@code DatagramSocketImpl} attached to this socket,
		''' creating it if necessary.
		''' </summary>
		''' <returns>  the {@code DatagramSocketImpl} attached to that
		'''          DatagramSocket </returns>
		''' <exception cref="SocketException"> if creation fails.
		''' @since 1.4 </exception>
		Friend Overridable Property impl As DatagramSocketImpl
			Get
				If Not created Then createImpl()
				Return impl
			End Get
		End Property

		''' <summary>
		''' Binds this DatagramSocket to a specific address and port.
		''' <p>
		''' If the address is {@code null}, then the system will pick up
		''' an ephemeral port and a valid local address to bind the socket.
		''' <p> </summary>
		''' <param name="addr"> The address and port to bind to. </param>
		''' <exception cref="SocketException"> if any error happens during the bind, or if the
		'''          socket is already bound. </exception>
		''' <exception cref="SecurityException">  if a security manager exists and its
		'''             {@code checkListen} method doesn't allow the operation. </exception>
		''' <exception cref="IllegalArgumentException"> if addr is a SocketAddress subclass
		'''         not supported by this socket.
		''' @since 1.4 </exception>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub bind(ByVal addr As SocketAddress)
			If closed Then Throw New SocketException("Socket is closed")
			If bound Then Throw New SocketException("already bound")
			If addr Is Nothing Then addr = New InetSocketAddress(0)
			If Not(TypeOf addr Is InetSocketAddress) Then Throw New IllegalArgumentException("Unsupported address type!")
			Dim epoint As InetSocketAddress = CType(addr, InetSocketAddress)
			If epoint.unresolved Then Throw New SocketException("Unresolved address")
			Dim iaddr As InetAddress = epoint.address
			Dim port_Renamed As Integer = epoint.port
			checkAddress(iaddr, "bind")
			Dim sec As SecurityManager = System.securityManager
			If sec IsNot Nothing Then sec.checkListen(port_Renamed)
			Try
				impl.bind(port_Renamed, iaddr)
			Catch e As SocketException
				impl.close()
				Throw e
			End Try
			bound = True
		End Sub

		Friend Overridable Sub checkAddress(ByVal addr As InetAddress, ByVal op As String)
			If addr Is Nothing Then Return
			If Not(TypeOf addr Is Inet4Address OrElse TypeOf addr Is Inet6Address) Then Throw New IllegalArgumentException(op & ": invalid address type")
		End Sub

		''' <summary>
		''' Connects the socket to a remote address for this socket. When a
		''' socket is connected to a remote address, packets may only be
		''' sent to or received from that address. By default a datagram
		''' socket is not connected.
		''' 
		''' <p>If the remote destination to which the socket is connected does not
		''' exist, or is otherwise unreachable, and if an ICMP destination unreachable
		''' packet has been received for that address, then a subsequent call to
		''' send or receive may throw a PortUnreachableException. Note, there is no
		''' guarantee that the exception will be thrown.
		''' 
		''' <p> If a security manager has been installed then it is invoked to check
		''' access to the remote address. Specifically, if the given {@code address}
		''' is a <seealso cref="InetAddress#isMulticastAddress multicast address"/>,
		''' the security manager's {@link
		''' java.lang.SecurityManager#checkMulticast(InetAddress)
		''' checkMulticast} method is invoked with the given {@code address}.
		''' Otherwise, the security manager's {@link
		''' java.lang.SecurityManager#checkConnect(String,int) checkConnect}
		''' and <seealso cref="java.lang.SecurityManager#checkAccept checkAccept"/> methods
		''' are invoked, with the given {@code address} and {@code port}, to
		''' verify that datagrams are permitted to be sent and received
		''' respectively.
		''' 
		''' <p> When a socket is connected, <seealso cref="#receive receive"/> and
		''' <seealso cref="#send send"/> <b>will not perform any security checks</b>
		''' on incoming and outgoing packets, other than matching the packet's
		''' and the socket's address and port. On a send operation, if the
		''' packet's address is set and the packet's address and the socket's
		''' address do not match, an {@code IllegalArgumentException} will be
		''' thrown. A socket connected to a multicast address may only be used
		''' to send packets.
		''' </summary>
		''' <param name="address"> the remote address for the socket
		''' </param>
		''' <param name="port"> the remote port for the socket.
		''' </param>
		''' <exception cref="IllegalArgumentException">
		'''         if the address is null, or the port is out of range.
		''' </exception>
		''' <exception cref="SecurityException">
		'''         if a security manager has been installed and it does
		'''         not permit access to the given remote address
		''' </exception>
		''' <seealso cref= #disconnect </seealso>
		Public Overridable Sub connect(ByVal address As InetAddress, ByVal port As Integer)
			Try
				connectInternal(address, port)
			Catch se As SocketException
				Throw New [Error]("connect failed", se)
			End Try
		End Sub

		''' <summary>
		''' Connects this socket to a remote socket address (IP address + port number).
		''' 
		''' <p> If given an <seealso cref="InetSocketAddress InetSocketAddress"/>, this method
		''' behaves as if invoking <seealso cref="#connect(InetAddress,int) connect(InetAddress,int)"/>
		''' with the the given socket addresses IP address and port number.
		''' </summary>
		''' <param name="addr">    The remote address.
		''' </param>
		''' <exception cref="SocketException">
		'''          if the connect fails
		''' </exception>
		''' <exception cref="IllegalArgumentException">
		'''         if {@code addr} is {@code null}, or {@code addr} is a SocketAddress
		'''         subclass not supported by this socket
		''' </exception>
		''' <exception cref="SecurityException">
		'''         if a security manager has been installed and it does
		'''         not permit access to the given remote address
		''' 
		''' @since 1.4 </exception>
		Public Overridable Sub connect(ByVal addr As SocketAddress)
			If addr Is Nothing Then Throw New IllegalArgumentException("Address can't be null")
			If Not(TypeOf addr Is InetSocketAddress) Then Throw New IllegalArgumentException("Unsupported address type")
			Dim epoint As InetSocketAddress = CType(addr, InetSocketAddress)
			If epoint.unresolved Then Throw New SocketException("Unresolved address")
			connectInternal(epoint.address, epoint.port)
		End Sub

		''' <summary>
		''' Disconnects the socket. If the socket is closed or not connected,
		''' then this method has no effect.
		''' </summary>
		''' <seealso cref= #connect </seealso>
		Public Overridable Sub disconnect()
			SyncLock Me
				If closed Then Return
				If connectState = ST_CONNECTED Then impl.disconnect()
				connectedAddress = Nothing
				connectedPort = -1
				connectState = ST_NOT_CONNECTED
				explicitFilter = False
			End SyncLock
		End Sub

		''' <summary>
		''' Returns the binding state of the socket.
		''' <p>
		''' If the socket was bound prior to being <seealso cref="#close closed"/>,
		''' then this method will continue to return {@code true}
		''' after the socket is closed.
		''' </summary>
		''' <returns> true if the socket successfully bound to an address
		''' @since 1.4 </returns>
		Public Overridable Property bound As Boolean
			Get
				Return bound
			End Get
		End Property

		''' <summary>
		''' Returns the connection state of the socket.
		''' <p>
		''' If the socket was connected prior to being <seealso cref="#close closed"/>,
		''' then this method will continue to return {@code true}
		''' after the socket is closed.
		''' </summary>
		''' <returns> true if the socket successfully connected to a server
		''' @since 1.4 </returns>
		Public Overridable Property connected As Boolean
			Get
				Return connectState <> ST_NOT_CONNECTED
			End Get
		End Property

		''' <summary>
		''' Returns the address to which this socket is connected. Returns
		''' {@code null} if the socket is not connected.
		''' <p>
		''' If the socket was connected prior to being <seealso cref="#close closed"/>,
		''' then this method will continue to return the connected address
		''' after the socket is closed.
		''' </summary>
		''' <returns> the address to which this socket is connected. </returns>
		Public Overridable Property inetAddress As InetAddress
			Get
				Return connectedAddress
			End Get
		End Property

		''' <summary>
		''' Returns the port number to which this socket is connected.
		''' Returns {@code -1} if the socket is not connected.
		''' <p>
		''' If the socket was connected prior to being <seealso cref="#close closed"/>,
		''' then this method will continue to return the connected port number
		''' after the socket is closed.
		''' </summary>
		''' <returns> the port number to which this socket is connected. </returns>
		Public Overridable Property port As Integer
			Get
				Return connectedPort
			End Get
		End Property

		''' <summary>
		''' Returns the address of the endpoint this socket is connected to, or
		''' {@code null} if it is unconnected.
		''' <p>
		''' If the socket was connected prior to being <seealso cref="#close closed"/>,
		''' then this method will continue to return the connected address
		''' after the socket is closed.
		''' </summary>
		''' <returns> a {@code SocketAddress} representing the remote
		'''         endpoint of this socket, or {@code null} if it is
		'''         not connected yet. </returns>
		''' <seealso cref= #getInetAddress() </seealso>
		''' <seealso cref= #getPort() </seealso>
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
		''' </summary>
		''' <returns> a {@code SocketAddress} representing the local endpoint of this
		'''         socket, or {@code null} if it is closed or not bound yet. </returns>
		''' <seealso cref= #getLocalAddress() </seealso>
		''' <seealso cref= #getLocalPort() </seealso>
		''' <seealso cref= #bind(SocketAddress)
		''' @since 1.4 </seealso>

		Public Overridable Property localSocketAddress As SocketAddress
			Get
				If closed Then Return Nothing
				If Not bound Then Return Nothing
				Return New InetSocketAddress(localAddress, localPort)
			End Get
		End Property

		''' <summary>
		''' Sends a datagram packet from this socket. The
		''' {@code DatagramPacket} includes information indicating the
		''' data to be sent, its length, the IP address of the remote host,
		''' and the port number on the remote host.
		''' 
		''' <p>If there is a security manager, and the socket is not currently
		''' connected to a remote address, this method first performs some
		''' security checks. First, if {@code p.getAddress().isMulticastAddress()}
		''' is true, this method calls the
		''' security manager's {@code checkMulticast} method
		''' with {@code p.getAddress()} as its argument.
		''' If the evaluation of that expression is false,
		''' this method instead calls the security manager's
		''' {@code checkConnect} method with arguments
		''' {@code p.getAddress().getHostAddress()} and
		''' {@code p.getPort()}. Each call to a security manager method
		''' could result in a SecurityException if the operation is not allowed.
		''' </summary>
		''' <param name="p">   the {@code DatagramPacket} to be sent.
		''' </param>
		''' <exception cref="IOException">  if an I/O error occurs. </exception>
		''' <exception cref="SecurityException">  if a security manager exists and its
		'''             {@code checkMulticast} or {@code checkConnect}
		'''             method doesn't allow the send. </exception>
		''' <exception cref="PortUnreachableException"> may be thrown if the socket is connected
		'''             to a currently unreachable destination. Note, there is no
		'''             guarantee that the exception will be thrown. </exception>
		''' <exception cref="java.nio.channels.IllegalBlockingModeException">
		'''             if this socket has an associated channel,
		'''             and the channel is in non-blocking mode. </exception>
		''' <exception cref="IllegalArgumentException"> if the socket is connected,
		'''             and connected address and packet address differ.
		''' </exception>
		''' <seealso cref=        java.net.DatagramPacket </seealso>
		''' <seealso cref=        SecurityManager#checkMulticast(InetAddress) </seealso>
		''' <seealso cref=        SecurityManager#checkConnect
		''' @revised 1.4
		''' @spec JSR-51 </seealso>
		Public Overridable Sub send(ByVal p As DatagramPacket)
			Dim packetAddress As InetAddress = Nothing
			SyncLock p
				If closed Then Throw New SocketException("Socket is closed")
				checkAddress(p.address, "send")
				If connectState = ST_NOT_CONNECTED Then
					' check the address is ok wiht the security manager on every send.
					Dim security As SecurityManager = System.securityManager

					' The reason you want to synchronize on datagram packet
					' is because you don't want an applet to change the address
					' while you are trying to send the packet for example
					' after the security check but before the send.
					If security IsNot Nothing Then
						If p.address.multicastAddress Then
							security.checkMulticast(p.address)
						Else
							security.checkConnect(p.address.hostAddress, p.port)
						End If
					End If
				Else
					' we're connected
					packetAddress = p.address
					If packetAddress Is Nothing Then
						p.address = connectedAddress
						p.port = connectedPort
					ElseIf ((Not packetAddress.Equals(connectedAddress))) OrElse p.port IsNot connectedPort Then
						Throw New IllegalArgumentException("connected address " & "and packet address" & " differ")
					End If
				End If
				' Check whether the socket is bound
				If Not bound Then bind(New InetSocketAddress(0))
				' call the  method to send
				impl.send(p)
			End SyncLock
		End Sub

		''' <summary>
		''' Receives a datagram packet from this socket. When this method
		''' returns, the {@code DatagramPacket}'s buffer is filled with
		''' the data received. The datagram packet also contains the sender's
		''' IP address, and the port number on the sender's machine.
		''' <p>
		''' This method blocks until a datagram is received. The
		''' {@code length} field of the datagram packet object contains
		''' the length of the received message. If the message is longer than
		''' the packet's length, the message is truncated.
		''' <p>
		''' If there is a security manager, a packet cannot be received if the
		''' security manager's {@code checkAccept} method
		''' does not allow it.
		''' </summary>
		''' <param name="p">   the {@code DatagramPacket} into which to place
		'''                 the incoming data. </param>
		''' <exception cref="IOException">  if an I/O error occurs. </exception>
		''' <exception cref="SocketTimeoutException">  if setSoTimeout was previously called
		'''                 and the timeout has expired. </exception>
		''' <exception cref="PortUnreachableException"> may be thrown if the socket is connected
		'''             to a currently unreachable destination. Note, there is no guarantee that the
		'''             exception will be thrown. </exception>
		''' <exception cref="java.nio.channels.IllegalBlockingModeException">
		'''             if this socket has an associated channel,
		'''             and the channel is in non-blocking mode. </exception>
		''' <seealso cref=        java.net.DatagramPacket </seealso>
		''' <seealso cref=        java.net.DatagramSocket
		''' @revised 1.4
		''' @spec JSR-51 </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub receive(ByVal p As DatagramPacket)
			SyncLock p
				If Not bound Then bind(New InetSocketAddress(0))
				If connectState = ST_NOT_CONNECTED Then
					' check the address is ok with the security manager before every recv.
					Dim security As SecurityManager = System.securityManager
					If security IsNot Nothing Then
						Do
							Dim peekAd As String = Nothing
							Dim peekPort As Integer = 0
							' peek at the packet to see who it is from.
							If Not oldImpl Then
								' We can use the new peekData() API
								Dim peekPacket As New DatagramPacket(New SByte(0){}, 1)
								peekPort = impl.peekData(peekPacket)
								peekAd = peekPacket.address.hostAddress
							Else
								Dim adr As New InetAddress
								peekPort = impl.peek(adr)
								peekAd = adr.hostAddress
							End If
							Try
								security.checkAccept(peekAd, peekPort)
								' security check succeeded - so now break
								' and recv the packet.
								Exit Do
							Catch se As SecurityException
								' Throw away the offending packet by consuming
								' it in a tmp buffer.
								Dim tmp As New DatagramPacket(New SByte(0){}, 1)
								impl.receive(tmp)

								' silently discard the offending packet
								' and continue: unknown/malicious
								' entities on nets should not make
								' runtime throw security exception and
								' disrupt the applet by sending random
								' datagram packets.
								Continue Do
							End Try
						Loop ' end of while
					End If
				End If
				Dim tmp As DatagramPacket = Nothing
				If (connectState = ST_CONNECTED_NO_IMPL) OrElse explicitFilter Then
					' We have to do the filtering the old fashioned way since
					' the native impl doesn't support connect or the connect
					' via the impl failed, or .. "explicitFilter" may be set when
					' a socket is connected via the impl, for a period of time
					' when packets from other sources might be queued on socket.
					Dim [stop] As Boolean = False
					Do While Not [stop]
						Dim peekAddress As InetAddress = Nothing
						Dim peekPort As Integer = -1
						' peek at the packet to see who it is from.
						If Not oldImpl Then
							' We can use the new peekData() API
							Dim peekPacket As New DatagramPacket(New SByte(0){}, 1)
							peekPort = impl.peekData(peekPacket)
							peekAddress = peekPacket.address
						Else
							' this api only works for IPv4
							peekAddress = New InetAddress
							peekPort = impl.peek(peekAddress)
						End If
						If ((Not connectedAddress.Equals(peekAddress))) OrElse (connectedPort <> peekPort) Then
							' throw the packet away and silently continue
							tmp = New DatagramPacket(New SByte(1023){}, 1024)
							impl.receive(tmp)
							If explicitFilter Then
								If checkFiltering(tmp) Then [stop] = True
							End If
						Else
							[stop] = True
						End If
					Loop
				End If
				' If the security check succeeds, or the datagram is
				' connected then receive the packet
				impl.receive(p)
				If explicitFilter AndAlso tmp Is Nothing Then checkFiltering(p)
			End SyncLock
		End Sub

		Private Function checkFiltering(ByVal p As DatagramPacket) As Boolean
			bytesLeftToFilter -= p.length
			If bytesLeftToFilter <= 0 OrElse impl.dataAvailable() <= 0 Then
				explicitFilter = False
				Return True
			End If
			Return False
		End Function

		''' <summary>
		''' Gets the local address to which the socket is bound.
		''' 
		''' <p>If there is a security manager, its
		''' {@code checkConnect} method is first called
		''' with the host address and {@code -1}
		''' as its arguments to see if the operation is allowed.
		''' </summary>
		''' <seealso cref= SecurityManager#checkConnect </seealso>
		''' <returns>  the local address to which the socket is bound,
		'''          {@code null} if the socket is closed, or
		'''          an {@code InetAddress} representing
		'''          <seealso cref="InetAddress#isAnyLocalAddress wildcard"/>
		'''          address if either the socket is not bound, or
		'''          the security manager {@code checkConnect}
		'''          method does not allow the operation
		''' @since   1.1 </returns>
		Public Overridable Property localAddress As InetAddress
			Get
				If closed Then Return Nothing
				Dim [in] As InetAddress = Nothing
				Try
					[in] = CType(impl.getOption(SocketOptions.SO_BINDADDR), InetAddress)
					If [in].anyLocalAddress Then [in] = InetAddress.anyLocalAddress()
					Dim s As SecurityManager = System.securityManager
					If s IsNot Nothing Then s.checkConnect([in].hostAddress, -1)
				Catch e As Exception
					[in] = InetAddress.anyLocalAddress() ' "0.0.0.0"
				End Try
				Return [in]
			End Get
		End Property

		''' <summary>
		''' Returns the port number on the local host to which this socket
		''' is bound.
		''' </summary>
		''' <returns>  the port number on the local host to which this socket is bound,
		'''            {@code -1} if the socket is closed, or
		'''            {@code 0} if it is not bound yet. </returns>
		Public Overridable Property localPort As Integer
			Get
				If closed Then Return -1
				Try
					Return impl.localPort
				Catch e As Exception
					Return 0
				End Try
			End Get
		End Property

		''' <summary>
		''' Enable/disable SO_TIMEOUT with the specified timeout, in
		'''  milliseconds. With this option set to a non-zero timeout,
		'''  a call to receive() for this DatagramSocket
		'''  will block for only this amount of time.  If the timeout expires,
		'''  a <B>java.net.SocketTimeoutException</B> is raised, though the
		'''  DatagramSocket is still valid.  The option <B>must</B> be enabled
		'''  prior to entering the blocking operation to have effect.  The
		'''  timeout must be {@code > 0}.
		'''  A timeout of zero is interpreted as an infinite timeout.
		''' </summary>
		''' <param name="timeout"> the specified timeout in milliseconds. </param>
		''' <exception cref="SocketException"> if there is an error in the underlying protocol, such as an UDP error.
		''' @since   JDK1.1 </exception>
		''' <seealso cref= #getSoTimeout() </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Property soTimeout As Integer
			Set(ByVal timeout As Integer)
				If closed Then Throw New SocketException("Socket is closed")
				impl.optionion(SocketOptions.SO_TIMEOUT, New Integer?(timeout))
			End Set
			Get
				If closed Then Throw New SocketException("Socket is closed")
				If impl Is Nothing Then Return 0
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
		''' Sets the SO_SNDBUF option to the specified value for this
		''' {@code DatagramSocket}. The SO_SNDBUF option is used by the
		''' network implementation as a hint to size the underlying
		''' network I/O buffers. The SO_SNDBUF setting may also be used
		''' by the network implementation to determine the maximum size
		''' of the packet that can be sent on this socket.
		''' <p>
		''' As SO_SNDBUF is a hint, applications that want to verify
		''' what size the buffer is should call <seealso cref="#getSendBufferSize()"/>.
		''' <p>
		''' Increasing the buffer size may allow multiple outgoing packets
		''' to be queued by the network implementation when the send rate
		''' is high.
		''' <p>
		''' Note: If <seealso cref="#send(DatagramPacket)"/> is used to send a
		''' {@code DatagramPacket} that is larger than the setting
		''' of SO_SNDBUF then it is implementation specific if the
		''' packet is sent or discarded.
		''' </summary>
		''' <param name="size"> the size to which to set the send buffer
		''' size. This value must be greater than 0.
		''' </param>
		''' <exception cref="SocketException"> if there is an error
		''' in the underlying protocol, such as an UDP error. </exception>
		''' <exception cref="IllegalArgumentException"> if the value is 0 or is
		''' negative. </exception>
		''' <seealso cref= #getSendBufferSize() </seealso>
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
		''' Sets the SO_RCVBUF option to the specified value for this
		''' {@code DatagramSocket}. The SO_RCVBUF option is used by the
		''' the network implementation as a hint to size the underlying
		''' network I/O buffers. The SO_RCVBUF setting may also be used
		''' by the network implementation to determine the maximum size
		''' of the packet that can be received on this socket.
		''' <p>
		''' Because SO_RCVBUF is a hint, applications that want to
		''' verify what size the buffers were set to should call
		''' <seealso cref="#getReceiveBufferSize()"/>.
		''' <p>
		''' Increasing SO_RCVBUF may allow the network implementation
		''' to buffer multiple packets when packets arrive faster than
		''' are being received using <seealso cref="#receive(DatagramPacket)"/>.
		''' <p>
		''' Note: It is implementation specific if a packet larger
		''' than SO_RCVBUF can be received.
		''' </summary>
		''' <param name="size"> the size to which to set the receive buffer
		''' size. This value must be greater than 0.
		''' </param>
		''' <exception cref="SocketException"> if there is an error in
		''' the underlying protocol, such as an UDP error. </exception>
		''' <exception cref="IllegalArgumentException"> if the value is 0 or is
		''' negative. </exception>
		''' <seealso cref= #getReceiveBufferSize() </seealso>
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
		''' Enable/disable the SO_REUSEADDR socket option.
		''' <p>
		''' For UDP sockets it may be necessary to bind more than one
		''' socket to the same socket address. This is typically for the
		''' purpose of receiving multicast packets
		''' (See <seealso cref="java.net.MulticastSocket"/>). The
		''' {@code SO_REUSEADDR} socket option allows multiple
		''' sockets to be bound to the same socket address if the
		''' {@code SO_REUSEADDR} socket option is enabled prior
		''' to binding the socket using <seealso cref="#bind(SocketAddress)"/>.
		''' <p>
		''' Note: This functionality is not supported by all existing platforms,
		''' so it is implementation specific whether this option will be ignored
		''' or not. However, if it is not supported then
		''' <seealso cref="#getReuseAddress()"/> will always return {@code false}.
		''' <p>
		''' When a {@code DatagramSocket} is created the initial setting
		''' of {@code SO_REUSEADDR} is disabled.
		''' <p>
		''' The behaviour when {@code SO_REUSEADDR} is enabled or
		''' disabled after a socket is bound (See <seealso cref="#isBound()"/>)
		''' is not defined.
		''' </summary>
		''' <param name="on">  whether to enable or disable the </param>
		''' <exception cref="SocketException"> if an error occurs enabling or
		'''            disabling the {@code SO_RESUEADDR} socket option,
		'''            or the socket is closed.
		''' @since 1.4 </exception>
		''' <seealso cref= #getReuseAddress() </seealso>
		''' <seealso cref= #bind(SocketAddress) </seealso>
		''' <seealso cref= #isBound() </seealso>
		''' <seealso cref= #isClosed() </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Property reuseAddress As Boolean
			Set(ByVal [on] As Boolean)
				If closed Then Throw New SocketException("Socket is closed")
				' Integer instead of Boolean for compatibility with older DatagramSocketImpl
				If oldImpl Then
					impl.optionion(SocketOptions.SO_REUSEADDR, New Integer?(If([on], -1, 0)))
				Else
					impl.optionion(SocketOptions.SO_REUSEADDR, Convert.ToBoolean([on]))
				End If
			End Set
			Get
				If closed Then Throw New SocketException("Socket is closed")
				Dim o As Object = impl.getOption(SocketOptions.SO_REUSEADDR)
				Return CBool(o)
			End Get
		End Property


		''' <summary>
		''' Enable/disable SO_BROADCAST.
		''' 
		''' <p> Some operating systems may require that the Java virtual machine be
		''' started with implementation specific privileges to enable this option or
		''' send broadcast datagrams.
		''' </summary>
		''' <param name="on">
		'''         whether or not to have broadcast turned on.
		''' </param>
		''' <exception cref="SocketException">
		'''          if there is an error in the underlying protocol, such as an UDP
		'''          error.
		''' 
		''' @since 1.4 </exception>
		''' <seealso cref= #getBroadcast() </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Property broadcast As Boolean
			Set(ByVal [on] As Boolean)
				If closed Then Throw New SocketException("Socket is closed")
				impl.optionion(SocketOptions.SO_BROADCAST, Convert.ToBoolean([on]))
			End Set
			Get
				If closed Then Throw New SocketException("Socket is closed")
				Return CBool(impl.getOption(SocketOptions.SO_BROADCAST))
			End Get
		End Property


		''' <summary>
		''' Sets traffic class or type-of-service octet in the IP
		''' datagram header for datagrams sent from this DatagramSocket.
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
		''' for Internet Protocol v6 {@code tc} is the value that
		''' would be placed into the sin6_flowinfo field of the IP header.
		''' </summary>
		''' <param name="tc">        an {@code int} value for the bitset. </param>
		''' <exception cref="SocketException"> if there is an error setting the
		''' traffic class or type-of-service
		''' @since 1.4 </exception>
		''' <seealso cref= #getTrafficClass </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
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
				If closed Then Throw New SocketException("Socket is closed")
				Return CInt(Fix(impl.getOption(SocketOptions.IP_TOS)))
			End Get
		End Property


		''' <summary>
		''' Closes this datagram socket.
		''' <p>
		''' Any thread currently blocked in <seealso cref="#receive"/> upon this socket
		''' will throw a <seealso cref="SocketException"/>.
		''' 
		''' <p> If this socket has an associated channel then the channel is closed
		''' as well.
		''' 
		''' @revised 1.4
		''' @spec JSR-51
		''' </summary>
		Public Overridable Sub close()
			SyncLock closeLock
				If closed Then Return
				impl.close()
				closed = True
			End SyncLock
		End Sub

		''' <summary>
		''' Returns whether the socket is closed or not.
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
		''' Returns the unique <seealso cref="java.nio.channels.DatagramChannel"/> object
		''' associated with this datagram socket, if any.
		''' 
		''' <p> A datagram socket will have a channel if, and only if, the channel
		''' itself was created via the {@link java.nio.channels.DatagramChannel#open
		''' DatagramChannel.open} method.
		''' </summary>
		''' <returns>  the datagram channel associated with this datagram socket,
		'''          or {@code null} if this socket was not created for a channel
		''' 
		''' @since 1.4
		''' @spec JSR-51 </returns>
		Public Overridable Property channel As java.nio.channels.DatagramChannel
			Get
				Return Nothing
			End Get
		End Property

		''' <summary>
		''' User defined factory for all datagram sockets.
		''' </summary>
		Friend Shared factory As DatagramSocketImplFactory

		''' <summary>
		''' Sets the datagram socket implementation factory for the
		''' application. The factory can be specified only once.
		''' <p>
		''' When an application creates a new datagram socket, the socket
		''' implementation factory's {@code createDatagramSocketImpl} method is
		''' called to create the actual datagram socket implementation.
		''' <p>
		''' Passing {@code null} to the method is a no-op unless the factory
		''' was already set.
		''' 
		''' <p>If there is a security manager, this method first calls
		''' the security manager's {@code checkSetFactory} method
		''' to ensure the operation is allowed.
		''' This could result in a SecurityException.
		''' </summary>
		''' <param name="fac">   the desired factory. </param>
		''' <exception cref="IOException">  if an I/O error occurs when setting the
		'''              datagram socket factory. </exception>
		''' <exception cref="SocketException">  if the factory is already defined. </exception>
		''' <exception cref="SecurityException">  if a security manager exists and its
		'''             {@code checkSetFactory} method doesn't allow the
		''' operation.
		''' @see
		''' java.net.DatagramSocketImplFactory#createDatagramSocketImpl() </exception>
		''' <seealso cref=       SecurityManager#checkSetFactory
		''' @since 1.3 </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Shared Property datagramSocketImplFactory As DatagramSocketImplFactory
			Set(ByVal fac As DatagramSocketImplFactory)
				If factory IsNot Nothing Then Throw New SocketException("factory already defined")
				Dim security As SecurityManager = System.securityManager
				If security IsNot Nothing Then security.checkSetFactory()
				factory = fac
			End Set
		End Property
	End Class

End Namespace