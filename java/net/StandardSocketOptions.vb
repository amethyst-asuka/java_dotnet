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

Namespace java.net

	''' <summary>
	''' Defines the <em>standard</em> socket options.
	''' 
	''' <p> The <seealso cref="SocketOption#name name"/> of each socket option defined by this
	''' class is its field name.
	''' 
	''' <p> In this release, the socket options defined here are used by {@link
	''' java.nio.channels.NetworkChannel network} channels in the {@link
	''' java.nio.channels channels} package.
	''' 
	''' @since 1.7
	''' </summary>

	Public NotInheritable Class StandardSocketOptions
		Private Sub New()
		End Sub

		' -- SOL_SOCKET --

		''' <summary>
		''' Allow transmission of broadcast datagrams.
		''' 
		''' <p> The value of this socket option is a {@code Boolean} that represents
		''' whether the option is enabled or disabled. The option is specific to
		''' datagram-oriented sockets sending to <seealso cref="java.net.Inet4Address IPv4"/>
		''' broadcast addresses. When the socket option is enabled then the socket
		''' can be used to send <em>broadcast datagrams</em>.
		''' 
		''' <p> The initial value of this socket option is {@code FALSE}. The socket
		''' option may be enabled or disabled at any time. Some operating systems may
		''' require that the Java virtual machine be started with implementation
		''' specific privileges to enable this option or send broadcast datagrams.
		''' </summary>
		''' <seealso cref= <a href="http://www.ietf.org/rfc/rfc919.txt">RFC&nbsp;929:
		''' Broadcasting Internet Datagrams</a> </seealso>
		''' <seealso cref= DatagramSocket#setBroadcast </seealso>
		Public Shared ReadOnly SO_BROADCAST As SocketOption(Of Boolean?) = New StdSocketOption(Of Boolean?)("SO_BROADCAST", GetType(Boolean))

		''' <summary>
		''' Keep connection alive.
		''' 
		''' <p> The value of this socket option is a {@code Boolean} that represents
		''' whether the option is enabled or disabled. When the {@code SO_KEEPALIVE}
		''' option is enabled the operating system may use a <em>keep-alive</em>
		''' mechanism to periodically probe the other end of a connection when the
		''' connection is otherwise idle. The exact semantics of the keep alive
		''' mechanism is system dependent and therefore unspecified.
		''' 
		''' <p> The initial value of this socket option is {@code FALSE}. The socket
		''' option may be enabled or disabled at any time.
		''' </summary>
		''' <seealso cref= <a href="http://www.ietf.org/rfc/rfc1122.txt">RFC&nbsp;1122
		''' Requirements for Internet Hosts -- Communication Layers</a> </seealso>
		''' <seealso cref= Socket#setKeepAlive </seealso>
		Public Shared ReadOnly SO_KEEPALIVE As SocketOption(Of Boolean?) = New StdSocketOption(Of Boolean?)("SO_KEEPALIVE", GetType(Boolean))

		''' <summary>
		''' The size of the socket send buffer.
		''' 
		''' <p> The value of this socket option is an {@code Integer} that is the
		''' size of the socket send buffer in bytes. The socket send buffer is an
		''' output buffer used by the networking implementation. It may need to be
		''' increased for high-volume connections. The value of the socket option is
		''' a <em>hint</em> to the implementation to size the buffer and the actual
		''' size may differ. The socket option can be queried to retrieve the actual
		''' size.
		''' 
		''' <p> For datagram-oriented sockets, the size of the send buffer may limit
		''' the size of the datagrams that may be sent by the socket. Whether
		''' datagrams larger than the buffer size are sent or discarded is system
		''' dependent.
		''' 
		''' <p> The initial/default size of the socket send buffer and the range of
		''' allowable values is system dependent although a negative size is not
		''' allowed. An attempt to set the socket send buffer to larger than its
		''' maximum size causes it to be set to its maximum size.
		''' 
		''' <p> An implementation allows this socket option to be set before the
		''' socket is bound or connected. Whether an implementation allows the
		''' socket send buffer to be changed after the socket is bound is system
		''' dependent.
		''' </summary>
		''' <seealso cref= Socket#setSendBufferSize </seealso>
		Public Shared ReadOnly SO_SNDBUF As SocketOption(Of Integer?) = New StdSocketOption(Of Integer?)("SO_SNDBUF", GetType(Integer))


		''' <summary>
		''' The size of the socket receive buffer.
		''' 
		''' <p> The value of this socket option is an {@code Integer} that is the
		''' size of the socket receive buffer in bytes. The socket receive buffer is
		''' an input buffer used by the networking implementation. It may need to be
		''' increased for high-volume connections or decreased to limit the possible
		''' backlog of incoming data. The value of the socket option is a
		''' <em>hint</em> to the implementation to size the buffer and the actual
		''' size may differ.
		''' 
		''' <p> For datagram-oriented sockets, the size of the receive buffer may
		''' limit the size of the datagrams that can be received. Whether datagrams
		''' larger than the buffer size can be received is system dependent.
		''' Increasing the socket receive buffer may be important for cases where
		''' datagrams arrive in bursts faster than they can be processed.
		''' 
		''' <p> In the case of stream-oriented sockets and the TCP/IP protocol, the
		''' size of the socket receive buffer may be used when advertising the size
		''' of the TCP receive window to the remote peer.
		''' 
		''' <p> The initial/default size of the socket receive buffer and the range
		''' of allowable values is system dependent although a negative size is not
		''' allowed. An attempt to set the socket receive buffer to larger than its
		''' maximum size causes it to be set to its maximum size.
		''' 
		''' <p> An implementation allows this socket option to be set before the
		''' socket is bound or connected. Whether an implementation allows the
		''' socket receive buffer to be changed after the socket is bound is system
		''' dependent.
		''' </summary>
		''' <seealso cref= <a href="http://www.ietf.org/rfc/rfc1323.txt">RFC&nbsp;1323: TCP
		''' Extensions for High Performance</a> </seealso>
		''' <seealso cref= Socket#setReceiveBufferSize </seealso>
		''' <seealso cref= ServerSocket#setReceiveBufferSize </seealso>
		Public Shared ReadOnly SO_RCVBUF As SocketOption(Of Integer?) = New StdSocketOption(Of Integer?)("SO_RCVBUF", GetType(Integer))

		''' <summary>
		''' Re-use address.
		''' 
		''' <p> The value of this socket option is a {@code Boolean} that represents
		''' whether the option is enabled or disabled. The exact semantics of this
		''' socket option are socket type and system dependent.
		''' 
		''' <p> In the case of stream-oriented sockets, this socket option will
		''' usually determine whether the socket can be bound to a socket address
		''' when a previous connection involving that socket address is in the
		''' <em>TIME_WAIT</em> state. On implementations where the semantics differ,
		''' and the socket option is not required to be enabled in order to bind the
		''' socket when a previous connection is in this state, then the
		''' implementation may choose to ignore this option.
		''' 
		''' <p> For datagram-oriented sockets the socket option is used to allow
		''' multiple programs bind to the same address. This option should be enabled
		''' when the socket is to be used for Internet Protocol (IP) multicasting.
		''' 
		''' <p> An implementation allows this socket option to be set before the
		''' socket is bound or connected. Changing the value of this socket option
		''' after the socket is bound has no effect. The default value of this
		''' socket option is system dependent.
		''' </summary>
		''' <seealso cref= <a href="http://www.ietf.org/rfc/rfc793.txt">RFC&nbsp;793: Transmission
		''' Control Protocol</a> </seealso>
		''' <seealso cref= ServerSocket#setReuseAddress </seealso>
		Public Shared ReadOnly SO_REUSEADDR As SocketOption(Of Boolean?) = New StdSocketOption(Of Boolean?)("SO_REUSEADDR", GetType(Boolean))

		''' <summary>
		''' Linger on close if data is present.
		''' 
		''' <p> The value of this socket option is an {@code Integer} that controls
		''' the action taken when unsent data is queued on the socket and a method
		''' to close the socket is invoked. If the value of the socket option is zero
		''' or greater, then it represents a timeout value, in seconds, known as the
		''' <em>linger interval</em>. The linger interval is the timeout for the
		''' {@code close} method to block while the operating system attempts to
		''' transmit the unsent data or it decides that it is unable to transmit the
		''' data. If the value of the socket option is less than zero then the option
		''' is disabled. In that case the {@code close} method does not wait until
		''' unsent data is transmitted; if possible the operating system will transmit
		''' any unsent data before the connection is closed.
		''' 
		''' <p> This socket option is intended for use with sockets that are configured
		''' in <seealso cref="java.nio.channels.SelectableChannel#isBlocking() blocking"/> mode
		''' only. The behavior of the {@code close} method when this option is
		''' enabled on a non-blocking socket is not defined.
		''' 
		''' <p> The initial value of this socket option is a negative value, meaning
		''' that the option is disabled. The option may be enabled, or the linger
		''' interval changed, at any time. The maximum value of the linger interval
		''' is system dependent. Setting the linger interval to a value that is
		''' greater than its maximum value causes the linger interval to be set to
		''' its maximum value.
		''' </summary>
		''' <seealso cref= Socket#setSoLinger </seealso>
		Public Shared ReadOnly SO_LINGER As SocketOption(Of Integer?) = New StdSocketOption(Of Integer?)("SO_LINGER", GetType(Integer))


		' -- IPPROTO_IP --

		''' <summary>
		''' The Type of Service (ToS) octet in the Internet Protocol (IP) header.
		''' 
		''' <p> The value of this socket option is an {@code Integer} representing
		''' the value of the ToS octet in IP packets sent by sockets to an {@link
		''' StandardProtocolFamily#INET IPv4} socket. The interpretation of the ToS
		''' octet is network specific and is not defined by this class. Further
		''' information on the ToS octet can be found in <a
		''' href="http://www.ietf.org/rfc/rfc1349.txt">RFC&nbsp;1349</a> and <a
		''' href="http://www.ietf.org/rfc/rfc2474.txt">RFC&nbsp;2474</a>. The value
		''' of the socket option is a <em>hint</em>. An implementation may ignore the
		''' value, or ignore specific values.
		''' 
		''' <p> The initial/default value of the TOS field in the ToS octet is
		''' implementation specific but will typically be {@code 0}. For
		''' datagram-oriented sockets the option may be configured at any time after
		''' the socket has been bound. The new value of the octet is used when sending
		''' subsequent datagrams. It is system dependent whether this option can be
		''' queried or changed prior to binding the socket.
		''' 
		''' <p> The behavior of this socket option on a stream-oriented socket, or an
		''' <seealso cref="StandardProtocolFamily#INET6 IPv6"/> socket, is not defined in this
		''' release.
		''' </summary>
		''' <seealso cref= DatagramSocket#setTrafficClass </seealso>
		Public Shared ReadOnly IP_TOS As SocketOption(Of Integer?) = New StdSocketOption(Of Integer?)("IP_TOS", GetType(Integer))

		''' <summary>
		''' The network interface for Internet Protocol (IP) multicast datagrams.
		''' 
		''' <p> The value of this socket option is a <seealso cref="NetworkInterface"/> that
		''' represents the outgoing interface for multicast datagrams sent by the
		''' datagram-oriented socket. For <seealso cref="StandardProtocolFamily#INET6 IPv6"/>
		''' sockets then it is system dependent whether setting this option also
		''' sets the outgoing interface for multicast datagrams sent to IPv4
		''' addresses.
		''' 
		''' <p> The initial/default value of this socket option may be {@code null}
		''' to indicate that outgoing interface will be selected by the operating
		''' system, typically based on the network routing tables. An implementation
		''' allows this socket option to be set after the socket is bound. Whether
		''' the socket option can be queried or changed prior to binding the socket
		''' is system dependent.
		''' </summary>
		''' <seealso cref= java.nio.channels.MulticastChannel </seealso>
		''' <seealso cref= MulticastSocket#setInterface </seealso>
		Public Shared ReadOnly IP_MULTICAST_IF As SocketOption(Of NetworkInterface) = New StdSocketOption(Of NetworkInterface)("IP_MULTICAST_IF", GetType(NetworkInterface))

		''' <summary>
		''' The <em>time-to-live</em> for Internet Protocol (IP) multicast datagrams.
		''' 
		''' <p> The value of this socket option is an {@code Integer} in the range
		''' {@code 0 <= value <= 255}. It is used to control the scope of multicast
		''' datagrams sent by the datagram-oriented socket.
		''' In the case of an <seealso cref="StandardProtocolFamily#INET IPv4"/> socket
		''' the option is the time-to-live (TTL) on multicast datagrams sent by the
		''' socket. Datagrams with a TTL of zero are not transmitted on the network
		''' but may be delivered locally. In the case of an {@link
		''' StandardProtocolFamily#INET6 IPv6} socket the option is the
		''' <em>hop limit</em> which is number of <em>hops</em> that the datagram can
		''' pass through before expiring on the network. For IPv6 sockets it is
		''' system dependent whether the option also sets the <em>time-to-live</em>
		''' on multicast datagrams sent to IPv4 addresses.
		''' 
		''' <p> The initial/default value of the time-to-live setting is typically
		''' {@code 1}. An implementation allows this socket option to be set after
		''' the socket is bound. Whether the socket option can be queried or changed
		''' prior to binding the socket is system dependent.
		''' </summary>
		''' <seealso cref= java.nio.channels.MulticastChannel </seealso>
		''' <seealso cref= MulticastSocket#setTimeToLive </seealso>
		Public Shared ReadOnly IP_MULTICAST_TTL As SocketOption(Of Integer?) = New StdSocketOption(Of Integer?)("IP_MULTICAST_TTL", GetType(Integer))

		''' <summary>
		''' Loopback for Internet Protocol (IP) multicast datagrams.
		''' 
		''' <p> The value of this socket option is a {@code Boolean} that controls
		''' the <em>loopback</em> of multicast datagrams. The value of the socket
		''' option represents if the option is enabled or disabled.
		''' 
		''' <p> The exact semantics of this socket options are system dependent.
		''' In particular, it is system dependent whether the loopback applies to
		''' multicast datagrams sent from the socket or received by the socket.
		''' For <seealso cref="StandardProtocolFamily#INET6 IPv6"/> sockets then it is
		''' system dependent whether the option also applies to multicast datagrams
		''' sent to IPv4 addresses.
		''' 
		''' <p> The initial/default value of this socket option is {@code TRUE}. An
		''' implementation allows this socket option to be set after the socket is
		''' bound. Whether the socket option can be queried or changed prior to
		''' binding the socket is system dependent.
		''' </summary>
		''' <seealso cref= java.nio.channels.MulticastChannel </seealso>
		'''  <seealso cref= MulticastSocket#setLoopbackMode </seealso>
		Public Shared ReadOnly IP_MULTICAST_LOOP As SocketOption(Of Boolean?) = New StdSocketOption(Of Boolean?)("IP_MULTICAST_LOOP", GetType(Boolean))


		' -- IPPROTO_TCP --

		''' <summary>
		''' Disable the Nagle algorithm.
		''' 
		''' <p> The value of this socket option is a {@code Boolean} that represents
		''' whether the option is enabled or disabled. The socket option is specific to
		''' stream-oriented sockets using the TCP/IP protocol. TCP/IP uses an algorithm
		''' known as <em>The Nagle Algorithm</em> to coalesce short segments and
		''' improve network efficiency.
		''' 
		''' <p> The default value of this socket option is {@code FALSE}. The
		''' socket option should only be enabled in cases where it is known that the
		''' coalescing impacts performance. The socket option may be enabled at any
		''' time. In other words, the Nagle Algorithm can be disabled. Once the option
		''' is enabled, it is system dependent whether it can be subsequently
		''' disabled. If it cannot, then invoking the {@code setOption} method to
		''' disable the option has no effect.
		''' </summary>
		''' <seealso cref= <a href="http://www.ietf.org/rfc/rfc1122.txt">RFC&nbsp;1122:
		''' Requirements for Internet Hosts -- Communication Layers</a> </seealso>
		''' <seealso cref= Socket#setTcpNoDelay </seealso>
		Public Shared ReadOnly TCP_NODELAY As SocketOption(Of Boolean?) = New StdSocketOption(Of Boolean?)("TCP_NODELAY", GetType(Boolean))


		Private Class StdSocketOption(Of T)
			Implements SocketOption(Of T)

			Private ReadOnly name_Renamed As String
			Private ReadOnly type_Renamed As  [Class]
			Friend Sub New(  name As String,   type As [Class])
				Me.name_Renamed = name
				Me.type_Renamed = type
			End Sub
			Public Overrides Function name() As String Implements SocketOption(Of T).name
				Return name_Renamed
			End Function
			Public Overrides Function type() As  [Class] Implements SocketOption(Of T).type
				Return type_Renamed
			End Function
			Public Overrides Function ToString() As String
				Return name_Renamed
			End Function
		End Class
	End Class

End Namespace