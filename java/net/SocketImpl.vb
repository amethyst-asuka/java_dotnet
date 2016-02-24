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
	''' The abstract class {@code SocketImpl} is a common superclass
	''' of all classes that actually implement sockets. It is used to
	''' create both client and server sockets.
	''' <p>
	''' A "plain" socket implements these methods exactly as
	''' described, without attempting to go through a firewall or proxy.
	''' 
	''' @author  unascribed
	''' @since   JDK1.0
	''' </summary>
	Public MustInherit Class SocketImpl
		Implements SocketOptions

			Public MustOverride Function getOption(ByVal optID As Integer) As Object Implements SocketOptions.getOption
			Public MustOverride Sub setOption(ByVal optID As Integer, ByVal value As Object) Implements SocketOptions.setOption
		''' <summary>
		''' The actual Socket object.
		''' </summary>
		Friend socket_Renamed As Socket = Nothing
		Friend serverSocket_Renamed As ServerSocket = Nothing

		''' <summary>
		''' The file descriptor object for this socket.
		''' </summary>
		Protected Friend fd As java.io.FileDescriptor

		''' <summary>
		''' The IP address of the remote end of this socket.
		''' </summary>
		Protected Friend address As InetAddress

		''' <summary>
		''' The port number on the remote host to which this socket is connected.
		''' </summary>
		Protected Friend port As Integer

		''' <summary>
		''' The local port number to which this socket is connected.
		''' </summary>
		Protected Friend localport As Integer

		''' <summary>
		''' Creates either a stream or a datagram socket.
		''' </summary>
		''' <param name="stream">   if {@code true}, create a stream socket;
		'''                      otherwise, create a datagram socket. </param>
		''' <exception cref="IOException">  if an I/O error occurs while creating the
		'''               socket. </exception>
		Protected Friend MustOverride Sub create(ByVal stream As Boolean)

		''' <summary>
		''' Connects this socket to the specified port on the named host.
		''' </summary>
		''' <param name="host">   the name of the remote host. </param>
		''' <param name="port">   the port number. </param>
		''' <exception cref="IOException">  if an I/O error occurs when connecting to the
		'''               remote host. </exception>
		Protected Friend MustOverride Sub connect(ByVal host As String, ByVal port As Integer)

		''' <summary>
		''' Connects this socket to the specified port number on the specified host.
		''' </summary>
		''' <param name="address">   the IP address of the remote host. </param>
		''' <param name="port">      the port number. </param>
		''' <exception cref="IOException">  if an I/O error occurs when attempting a
		'''               connection. </exception>
		Protected Friend MustOverride Sub connect(ByVal address As InetAddress, ByVal port As Integer)

		''' <summary>
		''' Connects this socket to the specified port number on the specified host.
		''' A timeout of zero is interpreted as an infinite timeout. The connection
		''' will then block until established or an error occurs.
		''' </summary>
		''' <param name="address">   the Socket address of the remote host. </param>
		''' <param name="timeout">  the timeout value, in milliseconds, or zero for no timeout. </param>
		''' <exception cref="IOException">  if an I/O error occurs when attempting a
		'''               connection.
		''' @since 1.4 </exception>
		Protected Friend MustOverride Sub connect(ByVal address As SocketAddress, ByVal timeout As Integer)

		''' <summary>
		''' Binds this socket to the specified local IP address and port number.
		''' </summary>
		''' <param name="host">   an IP address that belongs to a local interface. </param>
		''' <param name="port">   the port number. </param>
		''' <exception cref="IOException">  if an I/O error occurs when binding this socket. </exception>
		Protected Friend MustOverride Sub bind(ByVal host As InetAddress, ByVal port As Integer)

		''' <summary>
		''' Sets the maximum queue length for incoming connection indications
		''' (a request to connect) to the {@code count} argument. If a
		''' connection indication arrives when the queue is full, the
		''' connection is refused.
		''' </summary>
		''' <param name="backlog">   the maximum length of the queue. </param>
		''' <exception cref="IOException">  if an I/O error occurs when creating the queue. </exception>
		Protected Friend MustOverride Sub listen(ByVal backlog As Integer)

		''' <summary>
		''' Accepts a connection.
		''' </summary>
		''' <param name="s">   the accepted connection. </param>
		''' <exception cref="IOException">  if an I/O error occurs when accepting the
		'''               connection. </exception>
		Protected Friend MustOverride Sub accept(ByVal s As SocketImpl)

		''' <summary>
		''' Returns an input stream for this socket.
		''' </summary>
		''' <returns>     a stream for reading from this socket. </returns>
		''' <exception cref="IOException">  if an I/O error occurs when creating the
		'''               input stream. </exception>
		Protected Friend MustOverride ReadOnly Property inputStream As java.io.InputStream

		''' <summary>
		''' Returns an output stream for this socket.
		''' </summary>
		''' <returns>     an output stream for writing to this socket. </returns>
		''' <exception cref="IOException">  if an I/O error occurs when creating the
		'''               output stream. </exception>
		Protected Friend MustOverride ReadOnly Property outputStream As java.io.OutputStream

		''' <summary>
		''' Returns the number of bytes that can be read from this socket
		''' without blocking.
		''' </summary>
		''' <returns>     the number of bytes that can be read from this socket
		'''             without blocking. </returns>
		''' <exception cref="IOException">  if an I/O error occurs when determining the
		'''               number of bytes available. </exception>
		Protected Friend MustOverride Function available() As Integer

		''' <summary>
		''' Closes this socket.
		''' </summary>
		''' <exception cref="IOException">  if an I/O error occurs when closing this socket. </exception>
		Protected Friend MustOverride Sub close()

		''' <summary>
		''' Places the input stream for this socket at "end of stream".
		''' Any data sent to this socket is acknowledged and then
		''' silently discarded.
		''' 
		''' If you read from a socket input stream after invoking this method on the
		''' socket, the stream's {@code available} method will return 0, and its
		''' {@code read} methods will return {@code -1} (end of stream).
		''' </summary>
		''' <exception cref="IOException"> if an I/O error occurs when shutting down this
		''' socket. </exception>
		''' <seealso cref= java.net.Socket#shutdownOutput() </seealso>
		''' <seealso cref= java.net.Socket#close() </seealso>
		''' <seealso cref= java.net.Socket#setSoLinger(boolean, int)
		''' @since 1.3 </seealso>
		Protected Friend Overridable Sub shutdownInput()
		  Throw New java.io.IOException("Method not implemented!")
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
		''' socket. </exception>
		''' <seealso cref= java.net.Socket#shutdownInput() </seealso>
		''' <seealso cref= java.net.Socket#close() </seealso>
		''' <seealso cref= java.net.Socket#setSoLinger(boolean, int)
		''' @since 1.3 </seealso>
		Protected Friend Overridable Sub shutdownOutput()
		  Throw New java.io.IOException("Method not implemented!")
		End Sub

		''' <summary>
		''' Returns the value of this socket's {@code fd} field.
		''' </summary>
		''' <returns>  the value of this socket's {@code fd} field. </returns>
		''' <seealso cref=     java.net.SocketImpl#fd </seealso>
		Protected Friend Overridable Property fileDescriptor As java.io.FileDescriptor
			Get
				Return fd
			End Get
		End Property

		''' <summary>
		''' Returns the value of this socket's {@code address} field.
		''' </summary>
		''' <returns>  the value of this socket's {@code address} field. </returns>
		''' <seealso cref=     java.net.SocketImpl#address </seealso>
		Protected Friend Overridable Property inetAddress As InetAddress
			Get
				Return address
			End Get
		End Property

		''' <summary>
		''' Returns the value of this socket's {@code port} field.
		''' </summary>
		''' <returns>  the value of this socket's {@code port} field. </returns>
		''' <seealso cref=     java.net.SocketImpl#port </seealso>
		Protected Friend Overridable Property port As Integer
			Get
				Return port
			End Get
		End Property

		''' <summary>
		''' Returns whether or not this SocketImpl supports sending
		''' urgent data. By default, false is returned
		''' unless the method is overridden in a sub-class
		''' </summary>
		''' <returns>  true if urgent data supported </returns>
		''' <seealso cref=     java.net.SocketImpl#address
		''' @since 1.4 </seealso>
		Protected Friend Overridable Function supportsUrgentData() As Boolean
			Return False ' must be overridden in sub-class
		End Function

		''' <summary>
		''' Send one byte of urgent data on the socket.
		''' The byte to be sent is the low eight bits of the parameter </summary>
		''' <param name="data"> The byte of data to send </param>
		''' <exception cref="IOException"> if there is an error
		'''  sending the data.
		''' @since 1.4 </exception>
		Protected Friend MustOverride Sub sendUrgentData(ByVal data As Integer)

		''' <summary>
		''' Returns the value of this socket's {@code localport} field.
		''' </summary>
		''' <returns>  the value of this socket's {@code localport} field. </returns>
		''' <seealso cref=     java.net.SocketImpl#localport </seealso>
		Protected Friend Overridable Property localPort As Integer
			Get
				Return localport
			End Get
		End Property

		Friend Overridable Property socket As Socket
			Set(ByVal soc As Socket)
				Me.socket_Renamed = soc
			End Set
			Get
				Return socket_Renamed
			End Get
		End Property


		Friend Overridable Property serverSocket As ServerSocket
			Set(ByVal soc As ServerSocket)
				Me.serverSocket_Renamed = soc
			End Set
			Get
				Return serverSocket_Renamed
			End Get
		End Property


		''' <summary>
		''' Returns the address and port of this socket as a {@code String}.
		''' </summary>
		''' <returns>  a string representation of this socket. </returns>
		Public Overrides Function ToString() As String
			Return "Socket[addr=" & inetAddress & ",port=" & port & ",localport=" & localPort & "]"
		End Function

		Friend Overridable Sub reset()
			address = Nothing
			port = 0
			localport = 0
		End Sub

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
		''' By default, this method does nothing, unless it is overridden in a
		''' a sub-class.
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
		Protected Friend Overridable Sub setPerformancePreferences(ByVal connectionTime As Integer, ByVal latency As Integer, ByVal bandwidth As Integer)
			' Not implemented yet 
		End Sub

		 Friend Overridable Sub setOption(Of T)(ByVal name As SocketOption(Of T), ByVal value As T)
			If name Is StandardSocketOptions.SO_KEEPALIVE Then
				optionion(SocketOptions.SO_KEEPALIVE, value)
			ElseIf name Is StandardSocketOptions.SO_SNDBUF Then
				optionion(SocketOptions.SO_SNDBUF, value)
			ElseIf name Is StandardSocketOptions.SO_RCVBUF Then
				optionion(SocketOptions.SO_RCVBUF, value)
			ElseIf name Is StandardSocketOptions.SO_REUSEADDR Then
				optionion(SocketOptions.SO_REUSEADDR, value)
			ElseIf name Is StandardSocketOptions.SO_LINGER Then
				optionion(SocketOptions.SO_LINGER, value)
			ElseIf name Is StandardSocketOptions.IP_TOS Then
				optionion(SocketOptions.IP_TOS, value)
			ElseIf name Is StandardSocketOptions.TCP_NODELAY Then
				optionion(SocketOptions.TCP_NODELAY, value)
			Else
				Throw New UnsupportedOperationException("unsupported option")
			End If
		 End Sub

		 Friend Overridable Function getOption(Of T)(ByVal name As SocketOption(Of T)) As T
			If name Is StandardSocketOptions.SO_KEEPALIVE Then
				Return CType(getOption(SocketOptions.SO_KEEPALIVE), T)
			ElseIf name Is StandardSocketOptions.SO_SNDBUF Then
				Return CType(getOption(SocketOptions.SO_SNDBUF), T)
			ElseIf name Is StandardSocketOptions.SO_RCVBUF Then
				Return CType(getOption(SocketOptions.SO_RCVBUF), T)
			ElseIf name Is StandardSocketOptions.SO_REUSEADDR Then
				Return CType(getOption(SocketOptions.SO_REUSEADDR), T)
			ElseIf name Is StandardSocketOptions.SO_LINGER Then
				Return CType(getOption(SocketOptions.SO_LINGER), T)
			ElseIf name Is StandardSocketOptions.IP_TOS Then
				Return CType(getOption(SocketOptions.IP_TOS), T)
			ElseIf name Is StandardSocketOptions.TCP_NODELAY Then
				Return CType(getOption(SocketOptions.TCP_NODELAY), T)
			Else
				Throw New UnsupportedOperationException("unsupported option")
			End If
		 End Function
	End Class

End Namespace