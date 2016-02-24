Imports Microsoft.VisualBasic
Imports System

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

Namespace java.net


	''' <summary>
	''' Abstract datagram and multicast socket implementation base class.
	''' @author Pavani Diwanji
	''' @since  JDK1.1
	''' </summary>

	Public MustInherit Class DatagramSocketImpl
		Implements SocketOptions

			Public MustOverride Function getOption(ByVal optID As Integer) As Object Implements SocketOptions.getOption
			Public MustOverride Sub setOption(ByVal optID As Integer, ByVal value As Object) Implements SocketOptions.setOption

		''' <summary>
		''' The local port number.
		''' </summary>
		Protected Friend localPort As Integer

		''' <summary>
		''' The file descriptor object.
		''' </summary>
		Protected Friend fd As java.io.FileDescriptor

		Friend Overridable Function dataAvailable() As Integer
			' default impl returns zero, which disables the calling
			' functionality
			Return 0
		End Function

		''' <summary>
		''' The DatagramSocket or MulticastSocket
		''' that owns this impl
		''' </summary>
		Friend socket As DatagramSocket

		Friend Overridable Property datagramSocket As DatagramSocket
			Set(ByVal socket_Renamed As DatagramSocket)
				Me.socket = socket_Renamed
			End Set
			Get
				Return socket
			End Get
		End Property


		''' <summary>
		''' Creates a datagram socket. </summary>
		''' <exception cref="SocketException"> if there is an error in the
		''' underlying protocol, such as a TCP error. </exception>
		Protected Friend MustOverride Sub create()

		''' <summary>
		''' Binds a datagram socket to a local port and address. </summary>
		''' <param name="lport"> the local port </param>
		''' <param name="laddr"> the local address </param>
		''' <exception cref="SocketException"> if there is an error in the
		''' underlying protocol, such as a TCP error. </exception>
		Protected Friend MustOverride Sub bind(ByVal lport As Integer, ByVal laddr As InetAddress)

		''' <summary>
		''' Sends a datagram packet. The packet contains the data and the
		''' destination address to send the packet to. </summary>
		''' <param name="p"> the packet to be sent. </param>
		''' <exception cref="IOException"> if an I/O exception occurs while sending the
		''' datagram packet. </exception>
		''' <exception cref="PortUnreachableException"> may be thrown if the socket is connected
		''' to a currently unreachable destination. Note, there is no guarantee that
		''' the exception will be thrown. </exception>
		Protected Friend MustOverride Sub send(ByVal p As DatagramPacket)

		''' <summary>
		''' Connects a datagram socket to a remote destination. This associates the remote
		''' address with the local socket so that datagrams may only be sent to this destination
		''' and received from this destination. This may be overridden to call a native
		''' system connect.
		''' 
		''' <p>If the remote destination to which the socket is connected does not
		''' exist, or is otherwise unreachable, and if an ICMP destination unreachable
		''' packet has been received for that address, then a subsequent call to
		''' send or receive may throw a PortUnreachableException.
		''' Note, there is no guarantee that the exception will be thrown. </summary>
		''' <param name="address"> the remote InetAddress to connect to </param>
		''' <param name="port"> the remote port number </param>
		''' <exception cref="SocketException"> may be thrown if the socket cannot be
		''' connected to the remote destination
		''' @since 1.4 </exception>
		Protected Friend Overridable Sub connect(ByVal address As InetAddress, ByVal port As Integer)
		End Sub

		''' <summary>
		''' Disconnects a datagram socket from its remote destination.
		''' @since 1.4
		''' </summary>
		Protected Friend Overridable Sub disconnect()
		End Sub

		''' <summary>
		''' Peek at the packet to see who it is from. Updates the specified {@code InetAddress}
		''' to the address which the packet came from. </summary>
		''' <param name="i"> an InetAddress object </param>
		''' <returns> the port number which the packet came from. </returns>
		''' <exception cref="IOException"> if an I/O exception occurs </exception>
		''' <exception cref="PortUnreachableException"> may be thrown if the socket is connected
		'''       to a currently unreachable destination. Note, there is no guarantee that the
		'''       exception will be thrown. </exception>
		Protected Friend MustOverride Function peek(ByVal i As InetAddress) As Integer

		''' <summary>
		''' Peek at the packet to see who it is from. The data is copied into the specified
		''' {@code DatagramPacket}. The data is returned,
		''' but not consumed, so that a subsequent peekData/receive operation
		''' will see the same data. </summary>
		''' <param name="p"> the Packet Received. </param>
		''' <returns> the port number which the packet came from. </returns>
		''' <exception cref="IOException"> if an I/O exception occurs </exception>
		''' <exception cref="PortUnreachableException"> may be thrown if the socket is connected
		'''       to a currently unreachable destination. Note, there is no guarantee that the
		'''       exception will be thrown.
		''' @since 1.4 </exception>
		Protected Friend MustOverride Function peekData(ByVal p As DatagramPacket) As Integer
		''' <summary>
		''' Receive the datagram packet. </summary>
		''' <param name="p"> the Packet Received. </param>
		''' <exception cref="IOException"> if an I/O exception occurs
		''' while receiving the datagram packet. </exception>
		''' <exception cref="PortUnreachableException"> may be thrown if the socket is connected
		'''       to a currently unreachable destination. Note, there is no guarantee that the
		'''       exception will be thrown. </exception>
		Protected Friend MustOverride Sub receive(ByVal p As DatagramPacket)

		''' <summary>
		''' Set the TTL (time-to-live) option. </summary>
		''' <param name="ttl"> a byte specifying the TTL value
		''' </param>
		''' @deprecated use setTimeToLive instead. 
		''' <exception cref="IOException"> if an I/O exception occurs while setting
		''' the time-to-live option. </exception>
		''' <seealso cref= #getTTL() </seealso>
		<Obsolete("use setTimeToLive instead.")> _
		Protected Friend MustOverride Property tTL As SByte


		''' <summary>
		''' Set the TTL (time-to-live) option. </summary>
		''' <param name="ttl"> an {@code int} specifying the time-to-live value </param>
		''' <exception cref="IOException"> if an I/O exception occurs
		''' while setting the time-to-live option. </exception>
		''' <seealso cref= #getTimeToLive() </seealso>
		Protected Friend MustOverride Property timeToLive As Integer


		''' <summary>
		''' Join the multicast group. </summary>
		''' <param name="inetaddr"> multicast address to join. </param>
		''' <exception cref="IOException"> if an I/O exception occurs
		''' while joining the multicast group. </exception>
		Protected Friend MustOverride Sub join(ByVal inetaddr As InetAddress)

		''' <summary>
		''' Leave the multicast group. </summary>
		''' <param name="inetaddr"> multicast address to leave. </param>
		''' <exception cref="IOException"> if an I/O exception occurs
		''' while leaving the multicast group. </exception>
		Protected Friend MustOverride Sub leave(ByVal inetaddr As InetAddress)

		''' <summary>
		''' Join the multicast group. </summary>
		''' <param name="mcastaddr"> address to join. </param>
		''' <param name="netIf"> specifies the local interface to receive multicast
		'''        datagram packets </param>
		''' <exception cref="IOException"> if an I/O exception occurs while joining
		''' the multicast group
		''' @since 1.4 </exception>
		Protected Friend MustOverride Sub joinGroup(ByVal mcastaddr As SocketAddress, ByVal netIf As NetworkInterface)

		''' <summary>
		''' Leave the multicast group. </summary>
		''' <param name="mcastaddr"> address to leave. </param>
		''' <param name="netIf"> specified the local interface to leave the group at </param>
		''' <exception cref="IOException"> if an I/O exception occurs while leaving
		''' the multicast group
		''' @since 1.4 </exception>
		Protected Friend MustOverride Sub leaveGroup(ByVal mcastaddr As SocketAddress, ByVal netIf As NetworkInterface)

		''' <summary>
		''' Close the socket.
		''' </summary>
		Protected Friend MustOverride Sub close()

		''' <summary>
		''' Gets the local port. </summary>
		''' <returns> an {@code int} representing the local port value </returns>
		Protected Friend Overridable Property localPort As Integer
			Get
				Return localPort
			End Get
		End Property

		 Friend Overridable Sub setOption(Of T)(ByVal name As SocketOption(Of T), ByVal value As T)
			If name Is StandardSocketOptions.SO_SNDBUF Then
				optionion(SocketOptions.SO_SNDBUF, value)
			ElseIf name Is StandardSocketOptions.SO_RCVBUF Then
				optionion(SocketOptions.SO_RCVBUF, value)
			ElseIf name Is StandardSocketOptions.SO_REUSEADDR Then
				optionion(SocketOptions.SO_REUSEADDR, value)
			ElseIf name Is StandardSocketOptions.IP_TOS Then
				optionion(SocketOptions.IP_TOS, value)
			ElseIf name Is StandardSocketOptions.IP_MULTICAST_IF AndAlso (TypeOf datagramSocket Is MulticastSocket) Then
				optionion(SocketOptions.IP_MULTICAST_IF2, value)
			ElseIf name Is StandardSocketOptions.IP_MULTICAST_TTL AndAlso (TypeOf datagramSocket Is MulticastSocket) Then
				If Not(TypeOf value Is Integer?) Then Throw New IllegalArgumentException("not an integer")
				timeToLive = CInt(Fix(value))
			ElseIf name Is StandardSocketOptions.IP_MULTICAST_LOOP AndAlso (TypeOf datagramSocket Is MulticastSocket) Then
				optionion(SocketOptions.IP_MULTICAST_LOOP, value)
			Else
				Throw New UnsupportedOperationException("unsupported option")
			End If
		 End Sub

		 Friend Overridable Function getOption(Of T)(ByVal name As SocketOption(Of T)) As T
			If name Is StandardSocketOptions.SO_SNDBUF Then
				Return CType(getOption(SocketOptions.SO_SNDBUF), T)
			ElseIf name Is StandardSocketOptions.SO_RCVBUF Then
				Return CType(getOption(SocketOptions.SO_RCVBUF), T)
			ElseIf name Is StandardSocketOptions.SO_REUSEADDR Then
				Return CType(getOption(SocketOptions.SO_REUSEADDR), T)
			ElseIf name Is StandardSocketOptions.IP_TOS Then
				Return CType(getOption(SocketOptions.IP_TOS), T)
			ElseIf name Is StandardSocketOptions.IP_MULTICAST_IF AndAlso (TypeOf datagramSocket Is MulticastSocket) Then
				Return CType(getOption(SocketOptions.IP_MULTICAST_IF2), T)
			ElseIf name Is StandardSocketOptions.IP_MULTICAST_TTL AndAlso (TypeOf datagramSocket Is MulticastSocket) Then
				Dim ttl_Renamed As Integer? = timeToLive
				Return CType(ttl_Renamed, T)
			ElseIf name Is StandardSocketOptions.IP_MULTICAST_LOOP AndAlso (TypeOf datagramSocket Is MulticastSocket) Then
				Return CType(getOption(SocketOptions.IP_MULTICAST_LOOP), T)
			Else
				Throw New UnsupportedOperationException("unsupported option")
			End If
		 End Function

		''' <summary>
		''' Gets the datagram socket file descriptor. </summary>
		''' <returns> a {@code FileDescriptor} object representing the datagram socket
		''' file descriptor </returns>
		Protected Friend Overridable Property fileDescriptor As java.io.FileDescriptor
			Get
				Return fd
			End Get
		End Property
	End Class

End Namespace