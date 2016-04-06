Imports Microsoft.VisualBasic
Imports System
Imports System.Runtime.CompilerServices
Imports System.Runtime.InteropServices

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
	''' Note: This is not a public [Class], so that applets cannot call
	''' into the implementation directly and hence cannot bypass the
	''' security checks present in the DatagramSocket and MulticastSocket
	''' classes.
	''' 
	''' @author Pavani Diwanji
	''' </summary>

	Friend MustInherit Class AbstractPlainDatagramSocketImpl
		Inherits DatagramSocketImpl

		' timeout value for receive() 
		Friend timeout As Integer = 0
		Friend connected As Boolean = False
		Private trafficClass As Integer = 0
		Protected Friend connectedAddress As InetAddress = Nothing
		Private connectedPort As Integer = -1

		Private Shared ReadOnly os As String = java.security.AccessController.doPrivileged(New sun.security.action.GetPropertyAction("os.name")
	   Friend )

		''' <summary>
		''' flag set if the native connect() call not to be used
		''' </summary>
		Private Shared ReadOnly connectDisabled As Boolean = os.contains("OS X")

		''' <summary>
		''' Load net library into runtime.
		''' </summary>
		Shared Sub New()
			java.security.AccessController.doPrivileged(New PrivilegedActionAnonymousInnerClassHelper(Of T)
			init()
		End Sub

		Private Class PrivilegedActionAnonymousInnerClassHelper(Of T)
			Implements PrivilegedAction(Of T)

			Public Overridable Function run() As Void
'JAVA TO VB CONVERTER TODO TASK: The library is specified in the 'DllImport' attribute for .NET:
'				System.loadLibrary("net")
				Return Nothing
			End Function
		End Class

		''' <summary>
		''' Creates a datagram socket
		''' </summary>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Protected Friend Overrides Sub create()
			sun.net.ResourceManager.beforeUdpCreate()
			fd = New java.io.FileDescriptor
			Try
				datagramSocketCreate()
			Catch ioe As SocketException
				sun.net.ResourceManager.afterUdpClose()
				fd = Nothing
				Throw ioe
			End Try
		End Sub

		''' <summary>
		''' Binds a datagram socket to a local port.
		''' </summary>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Protected Friend Overrides Sub bind(  lport As Integer,   laddr As InetAddress)
			bind0(lport, laddr)
		End Sub

		Protected Friend MustOverride Sub bind0(  lport As Integer,   laddr As InetAddress)

		''' <summary>
		''' Sends a datagram packet. The packet contains the data and the
		''' destination address to send the packet to. </summary>
		''' <param name="p"> the packet to be sent. </param>
		Protected Friend MustOverride Sub send(  p As DatagramPacket)

		''' <summary>
		''' Connects a datagram socket to a remote destination. This associates the remote
		''' address with the local socket so that datagrams may only be sent to this destination
		''' and received from this destination. </summary>
		''' <param name="address"> the remote InetAddress to connect to </param>
		''' <param name="port"> the remote port number </param>
		Protected Friend Overrides Sub connect(  address As InetAddress,   port As Integer)
			connect0(address, port)
			connectedAddress = address
			connectedPort = port
			connected = True
		End Sub

		''' <summary>
		''' Disconnects a previously connected socket. Does nothing if the socket was
		''' not connected already.
		''' </summary>
		Protected Friend Overrides Sub disconnect()
			disconnect0(connectedAddress.holder().family)
			connected = False
			connectedAddress = Nothing
			connectedPort = -1
		End Sub

		''' <summary>
		''' Peek at the packet to see who it is from. </summary>
		''' <param name="i"> the address to populate with the sender address </param>
		Protected Friend MustOverride Function peek(  i As InetAddress) As Integer
		Protected Friend MustOverride Function peekData(  p As DatagramPacket) As Integer
		''' <summary>
		''' Receive the datagram packet. </summary>
		''' <param name="p"> the packet to receive into </param>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Protected Friend Overrides Sub receive(  p As DatagramPacket)
			receive0(p)
		End Sub

		Protected Friend MustOverride Sub receive0(  p As DatagramPacket)

		''' <summary>
		''' Set the TTL (time-to-live) option. </summary>
		''' <param name="ttl"> TTL to be set. </param>
		Protected Friend MustOverride Property timeToLive As Integer


		''' <summary>
		''' Set the TTL (time-to-live) option. </summary>
		''' <param name="ttl"> TTL to be set. </param>
		<Obsolete> _
		Protected Friend MustOverride Property tTL As SByte


		''' <summary>
		''' Join the multicast group. </summary>
		''' <param name="inetaddr"> multicast address to join. </param>
		Protected Friend Overrides Sub join(  inetaddr As InetAddress)
			join(inetaddr, Nothing)
		End Sub

		''' <summary>
		''' Leave the multicast group. </summary>
		''' <param name="inetaddr"> multicast address to leave. </param>
		Protected Friend Overrides Sub leave(  inetaddr As InetAddress)
			leave(inetaddr, Nothing)
		End Sub
		''' <summary>
		''' Join the multicast group. </summary>
		''' <param name="mcastaddr"> multicast address to join. </param>
		''' <param name="netIf"> specifies the local interface to receive multicast
		'''        datagram packets </param>
		''' <exception cref="IllegalArgumentException"> if mcastaddr is null or is a
		'''          SocketAddress subclass not supported by this socket
		''' @since 1.4 </exception>

		Protected Friend Overrides Sub joinGroup(  mcastaddr As SocketAddress,   netIf As NetworkInterface)
			If mcastaddr Is Nothing OrElse Not(TypeOf mcastaddr Is InetSocketAddress) Then Throw New IllegalArgumentException("Unsupported address type")
			join(CType(mcastaddr, InetSocketAddress).address, netIf)
		End Sub

		Protected Friend MustOverride Sub join(  inetaddr As InetAddress,   netIf As NetworkInterface)

		''' <summary>
		''' Leave the multicast group. </summary>
		''' <param name="mcastaddr">  multicast address to leave. </param>
		''' <param name="netIf"> specified the local interface to leave the group at </param>
		''' <exception cref="IllegalArgumentException"> if mcastaddr is null or is a
		'''          SocketAddress subclass not supported by this socket
		''' @since 1.4 </exception>
		Protected Friend Overrides Sub leaveGroup(  mcastaddr As SocketAddress,   netIf As NetworkInterface)
			If mcastaddr Is Nothing OrElse Not(TypeOf mcastaddr Is InetSocketAddress) Then Throw New IllegalArgumentException("Unsupported address type")
			leave(CType(mcastaddr, InetSocketAddress).address, netIf)
		End Sub

		Protected Friend MustOverride Sub leave(  inetaddr As InetAddress,   netIf As NetworkInterface)

		''' <summary>
		''' Close the socket.
		''' </summary>
		Protected Friend Overrides Sub close()
			If fd IsNot Nothing Then
				datagramSocketClose()
				sun.net.ResourceManager.afterUdpClose()
				fd = Nothing
			End If
		End Sub

		Protected Friend Overridable Property closed As Boolean
			Get
				Return If(fd Is Nothing, True, False)
			End Get
		End Property

		Protected Overrides Sub Finalize()
			close()
		End Sub

		''' <summary>
		''' set a value - since we only support (setting) binary options
		''' here, o must be a Boolean
		''' </summary>

		 Public Overrides Sub setOption(  optID As Integer,   o As Object)
			 If closed Then Throw New SocketException("Socket Closed")
			 Select Case optID
	'             check type safety b4 going native.  These should never
	'             * fail, since only java.Socket* has access to
	'             * PlainSocketImpl.setOption().
	'             
			 Case SO_TIMEOUT
				 If o Is Nothing OrElse Not(TypeOf o Is Integer?) Then Throw New SocketException("bad argument for SO_TIMEOUT")
				 Dim tmp As Integer = CInt(Fix(o))
				 If tmp < 0 Then Throw New IllegalArgumentException("timeout < 0")
				 timeout = tmp
				 Return
			 Case IP_TOS
				 If o Is Nothing OrElse Not(TypeOf o Is Integer?) Then Throw New SocketException("bad argument for IP_TOS")
				 trafficClass = CInt(Fix(o))
			 Case SO_REUSEADDR
				 If o Is Nothing OrElse Not(TypeOf o Is Boolean?) Then Throw New SocketException("bad argument for SO_REUSEADDR")
			 Case SO_BROADCAST
				 If o Is Nothing OrElse Not(TypeOf o Is Boolean?) Then Throw New SocketException("bad argument for SO_BROADCAST")
			 Case SO_BINDADDR
				 Throw New SocketException("Cannot re-bind Socket")
			 Case SO_RCVBUF, SO_SNDBUF
				 If o Is Nothing OrElse Not(TypeOf o Is Integer?) OrElse CInt(Fix(o)) < 0 Then Throw New SocketException("bad argument for SO_SNDBUF or " & "SO_RCVBUF")
			 Case IP_MULTICAST_IF
				 If o Is Nothing OrElse Not(TypeOf o Is InetAddress) Then Throw New SocketException("bad argument for IP_MULTICAST_IF")
			 Case IP_MULTICAST_IF2
				 If o Is Nothing OrElse Not(TypeOf o Is NetworkInterface) Then Throw New SocketException("bad argument for IP_MULTICAST_IF2")
			 Case IP_MULTICAST_LOOP
				 If o Is Nothing OrElse Not(TypeOf o Is Boolean?) Then Throw New SocketException("bad argument for IP_MULTICAST_LOOP")
			 Case Else
				 Throw New SocketException("invalid option: " & optID)
			 End Select
			 socketSetOption(optID, o)
		 End Sub

	'    
	'     * get option's state - set or not
	'     

		Public Overrides Function getOption(  optID As Integer) As Object
			If closed Then Throw New SocketException("Socket Closed")

			Dim result As Object

			Select Case optID
				Case SO_TIMEOUT
					result = New Integer?(timeout)

				Case IP_TOS
					result = socketGetOption(optID)
					If CInt(Fix(result)) = -1 Then result = New Integer?(trafficClass)

				Case SO_BINDADDR, IP_MULTICAST_IF, IP_MULTICAST_IF2, SO_RCVBUF, SO_SNDBUF, IP_MULTICAST_LOOP, SO_REUSEADDR, SO_BROADCAST
					result = socketGetOption(optID)

				Case Else
					Throw New SocketException("invalid option: " & optID)
			End Select

			Return result
		End Function

		Protected Friend MustOverride Sub datagramSocketCreate()
		Protected Friend MustOverride Sub datagramSocketClose()
		Protected Friend MustOverride Sub socketSetOption(  opt As Integer,   val As Object)
		Protected Friend MustOverride Function socketGetOption(  opt As Integer) As Object

		Protected Friend MustOverride Sub connect0(  address As InetAddress,   port As Integer)
		Protected Friend MustOverride Sub disconnect0(  family As Integer)

		Protected Friend Overridable Function nativeConnectDisabled() As Boolean
			Return connectDisabled
		End Function

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Friend Function dataAvailable() As Integer
		End Function
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Shared Sub init()
		End Sub
	End Class

End Namespace