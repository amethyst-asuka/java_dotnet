Imports System
Imports System.Collections.Generic

'
' * Copyright (c) 1995, 2014, Oracle and/or its affiliates. All rights reserved.
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
	''' The multicast datagram socket class is useful for sending
	''' and receiving IP multicast packets.  A MulticastSocket is
	''' a (UDP) DatagramSocket, with additional capabilities for
	''' joining "groups" of other multicast hosts on the internet.
	''' <P>
	''' A multicast group is specified by a class D IP address
	''' and by a standard UDP port number. Class D IP addresses
	''' are in the range <CODE>224.0.0.0</CODE> to <CODE>239.255.255.255</CODE>,
	''' inclusive. The address 224.0.0.0 is reserved and should not be used.
	''' <P>
	''' One would join a multicast group by first creating a MulticastSocket
	''' with the desired port, then invoking the
	''' <CODE>joinGroup(InetAddress groupAddr)</CODE>
	''' method:
	''' <PRE>
	''' // join a Multicast group and send the group salutations
	''' ...
	''' String msg = "Hello";
	''' InetAddress group = InetAddress.getByName("228.5.6.7");
	''' MulticastSocket s = new MulticastSocket(6789);
	''' s.joinGroup(group);
	''' DatagramPacket hi = new DatagramPacket(msg.getBytes(), msg.length(),
	'''                             group, 6789);
	''' s.send(hi);
	''' // get their responses!
	''' byte[] buf = new byte[1000];
	''' DatagramPacket recv = new DatagramPacket(buf, buf.length);
	''' s.receive(recv);
	''' ...
	''' // OK, I'm done talking - leave the group...
	''' s.leaveGroup(group);
	''' </PRE>
	''' 
	''' When one sends a message to a multicast group, <B>all</B> subscribing
	''' recipients to that host and port receive the message (within the
	''' time-to-live range of the packet, see below).  The socket needn't
	''' be a member of the multicast group to send messages to it.
	''' <P>
	''' When a socket subscribes to a multicast group/port, it receives
	''' datagrams sent by other hosts to the group/port, as do all other
	''' members of the group and port.  A socket relinquishes membership
	''' in a group by the leaveGroup(InetAddress addr) method.  <B>
	''' Multiple MulticastSocket's</B> may subscribe to a multicast group
	''' and port concurrently, and they will all receive group datagrams.
	''' <P>
	''' Currently applets are not allowed to use multicast sockets.
	''' 
	''' @author Pavani Diwanji
	''' @since  JDK1.1
	''' </summary>
	Public Class MulticastSocket
		Inherits DatagramSocket

		''' <summary>
		''' Used on some platforms to record if an outgoing interface
		''' has been set for this socket.
		''' </summary>
		Private interfaceSet As Boolean

		''' <summary>
		''' Create a multicast socket.
		''' 
		''' <p>If there is a security manager,
		''' its {@code checkListen} method is first called
		''' with 0 as its argument to ensure the operation is allowed.
		''' This could result in a SecurityException.
		''' <p>
		''' When the socket is created the
		''' <seealso cref="DatagramSocket#setReuseAddress(boolean)"/> method is
		''' called to enable the SO_REUSEADDR socket option.
		''' </summary>
		''' <exception cref="IOException"> if an I/O exception occurs
		''' while creating the MulticastSocket </exception>
		''' <exception cref="SecurityException">  if a security manager exists and its
		'''             {@code checkListen} method doesn't allow the operation. </exception>
		''' <seealso cref= SecurityManager#checkListen </seealso>
		''' <seealso cref= java.net.DatagramSocket#setReuseAddress(boolean) </seealso>
		Public Sub New()
			Me.New(New InetSocketAddress(0))
		End Sub

		''' <summary>
		''' Create a multicast socket and bind it to a specific port.
		''' 
		''' <p>If there is a security manager,
		''' its {@code checkListen} method is first called
		''' with the {@code port} argument
		''' as its argument to ensure the operation is allowed.
		''' This could result in a SecurityException.
		''' <p>
		''' When the socket is created the
		''' <seealso cref="DatagramSocket#setReuseAddress(boolean)"/> method is
		''' called to enable the SO_REUSEADDR socket option.
		''' </summary>
		''' <param name="port"> port to use </param>
		''' <exception cref="IOException"> if an I/O exception occurs
		''' while creating the MulticastSocket </exception>
		''' <exception cref="SecurityException">  if a security manager exists and its
		'''             {@code checkListen} method doesn't allow the operation. </exception>
		''' <seealso cref= SecurityManager#checkListen </seealso>
		''' <seealso cref= java.net.DatagramSocket#setReuseAddress(boolean) </seealso>
		Public Sub New(ByVal port As Integer)
			Me.New(New InetSocketAddress(port))
		End Sub

		''' <summary>
		''' Create a MulticastSocket bound to the specified socket address.
		''' <p>
		''' Or, if the address is {@code null}, create an unbound socket.
		''' 
		''' <p>If there is a security manager,
		''' its {@code checkListen} method is first called
		''' with the SocketAddress port as its argument to ensure the operation is allowed.
		''' This could result in a SecurityException.
		''' <p>
		''' When the socket is created the
		''' <seealso cref="DatagramSocket#setReuseAddress(boolean)"/> method is
		''' called to enable the SO_REUSEADDR socket option.
		''' </summary>
		''' <param name="bindaddr"> Socket address to bind to, or {@code null} for
		'''                 an unbound socket. </param>
		''' <exception cref="IOException"> if an I/O exception occurs
		''' while creating the MulticastSocket </exception>
		''' <exception cref="SecurityException">  if a security manager exists and its
		'''             {@code checkListen} method doesn't allow the operation. </exception>
		''' <seealso cref= SecurityManager#checkListen </seealso>
		''' <seealso cref= java.net.DatagramSocket#setReuseAddress(boolean)
		''' 
		''' @since 1.4 </seealso>
		Public Sub New(ByVal bindaddr As SocketAddress)
			MyBase.New(CType(Nothing, SocketAddress))

			' Enable SO_REUSEADDR before binding
			reuseAddress = True

			If bindaddr IsNot Nothing Then
				Try
					bind(bindaddr)
				Finally
					If Not bound Then close()
				End Try
			End If
		End Sub

		''' <summary>
		''' The lock on the socket's TTL. This is for set/getTTL and
		''' send(packet,ttl).
		''' </summary>
		Private ttlLock As New Object

		''' <summary>
		''' The lock on the socket's interface - used by setInterface
		''' and getInterface
		''' </summary>
		Private infLock As New Object

		''' <summary>
		''' The "last" interface set by setInterface on this MulticastSocket
		''' </summary>
		Private infAddress As InetAddress = Nothing


		''' <summary>
		''' Set the default time-to-live for multicast packets sent out
		''' on this {@code MulticastSocket} in order to control the
		''' scope of the multicasts.
		''' 
		''' <p>The ttl is an <b>unsigned</b> 8-bit quantity, and so <B>must</B> be
		''' in the range {@code 0 <= ttl <= 0xFF }.
		''' </summary>
		''' <param name="ttl"> the time-to-live </param>
		''' <exception cref="IOException"> if an I/O exception occurs
		''' while setting the default time-to-live value </exception>
		''' @deprecated use the setTimeToLive method instead, which uses
		''' <b>int</b> instead of <b>byte</b> as the type for ttl. 
		''' <seealso cref= #getTTL() </seealso>
		<Obsolete("use the setTimeToLive method instead, which uses")> _
		Public Overridable Property tTL As SByte
			Set(ByVal ttl As SByte)
				If closed Then Throw New SocketException("Socket is closed")
				impl.tTL = ttl
			End Set
			Get
				If closed Then Throw New SocketException("Socket is closed")
				Return impl.tTL
			End Get
		End Property

		''' <summary>
		''' Set the default time-to-live for multicast packets sent out
		''' on this {@code MulticastSocket} in order to control the
		''' scope of the multicasts.
		''' 
		''' <P> The ttl <B>must</B> be in the range {@code  0 <= ttl <=
		''' 255} or an {@code IllegalArgumentException} will be thrown.
		''' Multicast packets sent with a TTL of {@code 0} are not transmitted
		''' on the network but may be delivered locally.
		''' </summary>
		''' <param name="ttl">
		'''         the time-to-live
		''' </param>
		''' <exception cref="IOException">
		'''          if an I/O exception occurs while setting the
		'''          default time-to-live value
		''' </exception>
		''' <seealso cref= #getTimeToLive() </seealso>
		Public Overridable Property timeToLive As Integer
			Set(ByVal ttl As Integer)
				If ttl < 0 OrElse ttl > 255 Then Throw New IllegalArgumentException("ttl out of range")
				If closed Then Throw New SocketException("Socket is closed")
				impl.timeToLive = ttl
			End Set
			Get
				If closed Then Throw New SocketException("Socket is closed")
				Return impl.timeToLive
			End Get
		End Property



		''' <summary>
		''' Joins a multicast group. Its behavior may be affected by
		''' {@code setInterface} or {@code setNetworkInterface}.
		''' 
		''' <p>If there is a security manager, this method first
		''' calls its {@code checkMulticast} method
		''' with the {@code mcastaddr} argument
		''' as its argument.
		''' </summary>
		''' <param name="mcastaddr"> is the multicast address to join
		''' </param>
		''' <exception cref="IOException"> if there is an error joining
		''' or when the address is not a multicast address. </exception>
		''' <exception cref="SecurityException">  if a security manager exists and its
		''' {@code checkMulticast} method doesn't allow the join.
		''' </exception>
		''' <seealso cref= SecurityManager#checkMulticast(InetAddress) </seealso>
		Public Overridable Sub joinGroup(ByVal mcastaddr As InetAddress)
			If closed Then Throw New SocketException("Socket is closed")

			checkAddress(mcastaddr, "joinGroup")
			Dim security As SecurityManager = System.securityManager
			If security IsNot Nothing Then security.checkMulticast(mcastaddr)

			If Not mcastaddr.multicastAddress Then Throw New SocketException("Not a multicast address")

			''' <summary>
			''' required for some platforms where it's not possible to join
			''' a group without setting the interface first.
			''' </summary>
			Dim defaultInterface_Renamed As NetworkInterface = NetworkInterface.default

			If (Not interfaceSet) AndAlso defaultInterface_Renamed IsNot Nothing Then networkInterface = defaultInterface_Renamed

			impl.join(mcastaddr)
		End Sub

		''' <summary>
		''' Leave a multicast group. Its behavior may be affected by
		''' {@code setInterface} or {@code setNetworkInterface}.
		''' 
		''' <p>If there is a security manager, this method first
		''' calls its {@code checkMulticast} method
		''' with the {@code mcastaddr} argument
		''' as its argument.
		''' </summary>
		''' <param name="mcastaddr"> is the multicast address to leave </param>
		''' <exception cref="IOException"> if there is an error leaving
		''' or when the address is not a multicast address. </exception>
		''' <exception cref="SecurityException">  if a security manager exists and its
		''' {@code checkMulticast} method doesn't allow the operation.
		''' </exception>
		''' <seealso cref= SecurityManager#checkMulticast(InetAddress) </seealso>
		Public Overridable Sub leaveGroup(ByVal mcastaddr As InetAddress)
			If closed Then Throw New SocketException("Socket is closed")

			checkAddress(mcastaddr, "leaveGroup")
			Dim security As SecurityManager = System.securityManager
			If security IsNot Nothing Then security.checkMulticast(mcastaddr)

			If Not mcastaddr.multicastAddress Then Throw New SocketException("Not a multicast address")

			impl.leave(mcastaddr)
		End Sub

		''' <summary>
		''' Joins the specified multicast group at the specified interface.
		''' 
		''' <p>If there is a security manager, this method first
		''' calls its {@code checkMulticast} method
		''' with the {@code mcastaddr} argument
		''' as its argument.
		''' </summary>
		''' <param name="mcastaddr"> is the multicast address to join </param>
		''' <param name="netIf"> specifies the local interface to receive multicast
		'''        datagram packets, or <i>null</i> to defer to the interface set by
		'''       <seealso cref="MulticastSocket#setInterface(InetAddress)"/> or
		'''       <seealso cref="MulticastSocket#setNetworkInterface(NetworkInterface)"/>
		''' </param>
		''' <exception cref="IOException"> if there is an error joining
		''' or when the address is not a multicast address. </exception>
		''' <exception cref="SecurityException">  if a security manager exists and its
		''' {@code checkMulticast} method doesn't allow the join. </exception>
		''' <exception cref="IllegalArgumentException"> if mcastaddr is null or is a
		'''          SocketAddress subclass not supported by this socket
		''' </exception>
		''' <seealso cref= SecurityManager#checkMulticast(InetAddress)
		''' @since 1.4 </seealso>
		Public Overridable Sub joinGroup(ByVal mcastaddr As SocketAddress, ByVal netIf As NetworkInterface)
			If closed Then Throw New SocketException("Socket is closed")

			If mcastaddr Is Nothing OrElse Not(TypeOf mcastaddr Is InetSocketAddress) Then Throw New IllegalArgumentException("Unsupported address type")

			If oldImpl Then Throw New UnsupportedOperationException

			checkAddress(CType(mcastaddr, InetSocketAddress).address, "joinGroup")
			Dim security As SecurityManager = System.securityManager
			If security IsNot Nothing Then security.checkMulticast(CType(mcastaddr, InetSocketAddress).address)

			If Not CType(mcastaddr, InetSocketAddress).address.multicastAddress Then Throw New SocketException("Not a multicast address")

			impl.joinGroup(mcastaddr, netIf)
		End Sub

		''' <summary>
		''' Leave a multicast group on a specified local interface.
		''' 
		''' <p>If there is a security manager, this method first
		''' calls its {@code checkMulticast} method
		''' with the {@code mcastaddr} argument
		''' as its argument.
		''' </summary>
		''' <param name="mcastaddr"> is the multicast address to leave </param>
		''' <param name="netIf"> specifies the local interface or <i>null</i> to defer
		'''             to the interface set by
		'''             <seealso cref="MulticastSocket#setInterface(InetAddress)"/> or
		'''             <seealso cref="MulticastSocket#setNetworkInterface(NetworkInterface)"/> </param>
		''' <exception cref="IOException"> if there is an error leaving
		''' or when the address is not a multicast address. </exception>
		''' <exception cref="SecurityException">  if a security manager exists and its
		''' {@code checkMulticast} method doesn't allow the operation. </exception>
		''' <exception cref="IllegalArgumentException"> if mcastaddr is null or is a
		'''          SocketAddress subclass not supported by this socket
		''' </exception>
		''' <seealso cref= SecurityManager#checkMulticast(InetAddress)
		''' @since 1.4 </seealso>
		Public Overridable Sub leaveGroup(ByVal mcastaddr As SocketAddress, ByVal netIf As NetworkInterface)
			If closed Then Throw New SocketException("Socket is closed")

			If mcastaddr Is Nothing OrElse Not(TypeOf mcastaddr Is InetSocketAddress) Then Throw New IllegalArgumentException("Unsupported address type")

			If oldImpl Then Throw New UnsupportedOperationException

			checkAddress(CType(mcastaddr, InetSocketAddress).address, "leaveGroup")
			Dim security As SecurityManager = System.securityManager
			If security IsNot Nothing Then security.checkMulticast(CType(mcastaddr, InetSocketAddress).address)

			If Not CType(mcastaddr, InetSocketAddress).address.multicastAddress Then Throw New SocketException("Not a multicast address")

			impl.leaveGroup(mcastaddr, netIf)
		End Sub

		''' <summary>
		''' Set the multicast network interface used by methods
		''' whose behavior would be affected by the value of the
		''' network interface. Useful for multihomed hosts. </summary>
		''' <param name="inf"> the InetAddress </param>
		''' <exception cref="SocketException"> if there is an error in
		''' the underlying protocol, such as a TCP error. </exception>
		''' <seealso cref= #getInterface() </seealso>
		Public Overridable Property [interface] As InetAddress
			Set(ByVal inf As InetAddress)
				If closed Then Throw New SocketException("Socket is closed")
				checkAddress(inf, "setInterface")
				SyncLock infLock
					impl.optionion(SocketOptions.IP_MULTICAST_IF, inf)
					infAddress = inf
					interfaceSet = True
				End SyncLock
			End Set
			Get
				If closed Then Throw New SocketException("Socket is closed")
				SyncLock infLock
					Dim ia As InetAddress = CType(impl.getOption(SocketOptions.IP_MULTICAST_IF), InetAddress)
    
					''' <summary>
					''' No previous setInterface or interface can be
					''' set using setNetworkInterface
					''' </summary>
					If infAddress Is Nothing Then Return ia
    
					''' <summary>
					''' Same interface set with setInterface?
					''' </summary>
					If ia.Equals(infAddress) Then Return ia
    
					''' <summary>
					''' Different InetAddress from what we set with setInterface
					''' so enumerate the current interface to see if the
					''' address set by setInterface is bound to this interface.
					''' </summary>
					Try
						Dim ni As NetworkInterface = NetworkInterface.getByInetAddress(ia)
						Dim addrs As System.Collections.IEnumerator(Of InetAddress) = ni.inetAddresses
	'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
						Do While addrs.hasMoreElements()
	'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
							Dim addr As InetAddress = addrs.nextElement()
							If addr.Equals(infAddress) Then Return infAddress
						Loop
    
						''' <summary>
						''' No match so reset infAddress to indicate that the
						''' interface has changed via means
						''' </summary>
						infAddress = Nothing
						Return ia
					Catch e As Exception
						Return ia
					End Try
				End SyncLock
			End Get
		End Property


		''' <summary>
		''' Specify the network interface for outgoing multicast datagrams
		''' sent on this socket.
		''' </summary>
		''' <param name="netIf"> the interface </param>
		''' <exception cref="SocketException"> if there is an error in
		''' the underlying protocol, such as a TCP error. </exception>
		''' <seealso cref= #getNetworkInterface()
		''' @since 1.4 </seealso>
		Public Overridable Property networkInterface As NetworkInterface
			Set(ByVal netIf As NetworkInterface)
    
				SyncLock infLock
					impl.optionion(SocketOptions.IP_MULTICAST_IF2, netIf)
					infAddress = Nothing
					interfaceSet = True
				End SyncLock
			End Set
			Get
				Dim ni As NetworkInterface = CType(impl.getOption(SocketOptions.IP_MULTICAST_IF2), NetworkInterface)
				If (ni.index = 0) OrElse (ni.index = -1) Then
					Dim addrs As InetAddress() = New InetAddress(0){}
					addrs(0) = InetAddress.anyLocalAddress()
					Return New NetworkInterface(addrs(0).hostName, 0, addrs)
				Else
					Return ni
				End If
			End Get
		End Property


		''' <summary>
		''' Disable/Enable local loopback of multicast datagrams
		''' The option is used by the platform's networking code as a hint
		''' for setting whether multicast data will be looped back to
		''' the local socket.
		''' 
		''' <p>Because this option is a hint, applications that want to
		''' verify what loopback mode is set to should call
		''' <seealso cref="#getLoopbackMode()"/> </summary>
		''' <param name="disable"> {@code true} to disable the LoopbackMode </param>
		''' <exception cref="SocketException"> if an error occurs while setting the value
		''' @since 1.4 </exception>
		''' <seealso cref= #getLoopbackMode </seealso>
		Public Overridable Property loopbackMode As Boolean
			Set(ByVal disable As Boolean)
				impl.optionion(SocketOptions.IP_MULTICAST_LOOP, Convert.ToBoolean(disable))
			End Set
			Get
				Return CBool(impl.getOption(SocketOptions.IP_MULTICAST_LOOP))
			End Get
		End Property


		''' <summary>
		''' Sends a datagram packet to the destination, with a TTL (time-
		''' to-live) other than the default for the socket.  This method
		''' need only be used in instances where a particular TTL is desired;
		''' otherwise it is preferable to set a TTL once on the socket, and
		''' use that default TTL for all packets.  This method does <B>not
		''' </B> alter the default TTL for the socket. Its behavior may be
		''' affected by {@code setInterface}.
		''' 
		''' <p>If there is a security manager, this method first performs some
		''' security checks. First, if {@code p.getAddress().isMulticastAddress()}
		''' is true, this method calls the
		''' security manager's {@code checkMulticast} method
		''' with {@code p.getAddress()} and {@code ttl} as its arguments.
		''' If the evaluation of that expression is false,
		''' this method instead calls the security manager's
		''' {@code checkConnect} method with arguments
		''' {@code p.getAddress().getHostAddress()} and
		''' {@code p.getPort()}. Each call to a security manager method
		''' could result in a SecurityException if the operation is not allowed.
		''' </summary>
		''' <param name="p"> is the packet to be sent. The packet should contain
		''' the destination multicast ip address and the data to be sent.
		''' One does not need to be the member of the group to send
		''' packets to a destination multicast address. </param>
		''' <param name="ttl"> optional time to live for multicast packet.
		''' default ttl is 1.
		''' </param>
		''' <exception cref="IOException"> is raised if an error occurs i.e
		''' error while setting ttl. </exception>
		''' <exception cref="SecurityException">  if a security manager exists and its
		'''             {@code checkMulticast} or {@code checkConnect}
		'''             method doesn't allow the send.
		''' </exception>
		''' @deprecated Use the following code or its equivalent instead:
		'''  ......
		'''  int ttl = mcastSocket.getTimeToLive();
		'''  mcastSocket.setTimeToLive(newttl);
		'''  mcastSocket.send(p);
		'''  mcastSocket.setTimeToLive(ttl);
		'''  ......
		''' 
		''' <seealso cref= DatagramSocket#send </seealso>
		''' <seealso cref= DatagramSocket#receive </seealso>
		''' <seealso cref= SecurityManager#checkMulticast(java.net.InetAddress, byte) </seealso>
		''' <seealso cref= SecurityManager#checkConnect </seealso>
		<Obsolete("Use the following code or its equivalent instead:")> _
		Public Overridable Sub send(ByVal p As DatagramPacket, ByVal ttl As SByte)
				If closed Then Throw New SocketException("Socket is closed")
				checkAddress(p.address, "send")
				SyncLock ttlLock
					SyncLock p
						If connectState = ST_NOT_CONNECTED Then
							' Security manager makes sure that the multicast address
							' is allowed one and that the ttl used is less
							' than the allowed maxttl.
							Dim security As SecurityManager = System.securityManager
							If security IsNot Nothing Then
								If p.address.multicastAddress Then
									security.checkMulticast(p.address, ttl)
								Else
									security.checkConnect(p.address.hostAddress, p.port)
								End If
							End If
						Else
							' we're connected
							Dim packetAddress As InetAddress = Nothing
							packetAddress = p.address
							If packetAddress Is Nothing Then
								p.address = connectedAddress
								p.port = connectedPort
							ElseIf ((Not packetAddress.Equals(connectedAddress))) OrElse p.port IsNot connectedPort Then
								Throw New SecurityException("connected address and packet address" & " differ")
							End If
						End If
						Dim dttl As SByte = tTL
						Try
							If ttl <> dttl Then impl.tTL = ttl
							' call the datagram method to send
							impl.send(p)
						Finally
							' set it back to default
							If ttl <> dttl Then impl.tTL = dttl
						End Try
					End SyncLock ' synch p
				End SyncLock 'synch ttl
		End Sub 'method
	End Class

End Namespace