Imports Microsoft.VisualBasic
Imports System
Imports System.Runtime.CompilerServices
Imports System.Runtime.InteropServices

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
	''' This class defines the plain DatagramSocketImpl that is used on
	''' Windows platforms greater than or equal to Windows Vista. These
	''' platforms have a dual layer TCP/IP stack and can handle both IPv4
	''' and IPV6 through a single file descriptor.
	''' <p>
	''' Note: Multicasting on a dual layer TCP/IP stack is always done with
	''' TwoStacksPlainDatagramSocketImpl. This is to overcome the lack
	''' of behavior defined for multicasting over a dual layer socket by the RFC.
	''' 
	''' @author Chris Hegarty
	''' </summary>

	Friend Class DualStackPlainDatagramSocketImpl
		Inherits AbstractPlainDatagramSocketImpl

		Friend Shared fdAccess As sun.misc.JavaIOFileDescriptorAccess = sun.misc.SharedSecrets.javaIOFileDescriptorAccess

		' true if this socket is exclusively bound
		Private ReadOnly exclusiveBind As Boolean

	'    
	'     * Set to true if SO_REUSEADDR is set after the socket is bound to
	'     * indicate SO_REUSEADDR is being emulated
	'     
		Private reuseAddressEmulated As Boolean

		' emulates SO_REUSEADDR when exclusiveBind is true and socket is bound
		Private isReuseAddress As Boolean

		Friend Sub New(  exclBind As Boolean)
			exclusiveBind = exclBind
		End Sub

		Protected Friend Overrides Sub datagramSocketCreate()
			If fd Is Nothing Then Throw New SocketException("Socket closed")

			Dim newfd As Integer = socketCreate(False) ' v6Only

			fdAccess.set(fd, newfd)
		End Sub

		<MethodImpl(MethodImplOptions.Synchronized)> _
		Protected Friend Overrides Sub bind0(  lport As Integer,   laddr As InetAddress)
			Dim nativefd As Integer = checkAndReturnNativeFD()

			If laddr Is Nothing Then Throw New NullPointerException("argument address")

			socketBind(nativefd, laddr, lport, exclusiveBind)
			If lport = 0 Then
				localPort = socketLocalPort(nativefd)
			Else
				localPort = lport
			End If
		End Sub

		<MethodImpl(MethodImplOptions.Synchronized)> _
		Protected Friend Overrides Function peek(  address As InetAddress) As Integer
			Dim nativefd As Integer = checkAndReturnNativeFD()

			If address Is Nothing Then Throw New NullPointerException("Null address in peek()")

			' Use peekData()
			Dim peekPacket As New DatagramPacket(New SByte(0){}, 1)
			Dim peekPort As Integer = peekData(peekPacket)
			address = peekPacket.address
			Return peekPort
		End Function

		<MethodImpl(MethodImplOptions.Synchronized)> _
		Protected Friend Overrides Function peekData(  p As DatagramPacket) As Integer
			Dim nativefd As Integer = checkAndReturnNativeFD()

			If p Is Nothing Then Throw New NullPointerException("packet")
			If p.data Is Nothing Then Throw New NullPointerException("packet buffer")

			Return socketReceiveOrPeekData(nativefd, p, timeout, connected, True) 'peek
		End Function

		<MethodImpl(MethodImplOptions.Synchronized)> _
		Protected Friend Overrides Sub receive0(  p As DatagramPacket)
			Dim nativefd As Integer = checkAndReturnNativeFD()

			If p Is Nothing Then Throw New NullPointerException("packet")
			If p.data Is Nothing Then Throw New NullPointerException("packet buffer")

			socketReceiveOrPeekData(nativefd, p, timeout, connected, False) 'receive
		End Sub

		Protected Friend Overrides Sub send(  p As DatagramPacket)
			Dim nativefd As Integer = checkAndReturnNativeFD()

			If p Is Nothing Then Throw New NullPointerException("null packet")

			If p.address Is Nothing OrElse p.data Is Nothing Then Throw New NullPointerException("null address || null buffer")

			socketSend(nativefd, p.data, p.offset, p.length, p.address, p.port, connected)
		End Sub

		Protected Friend Overrides Sub connect0(  address As InetAddress,   port As Integer)
			Dim nativefd As Integer = checkAndReturnNativeFD()

			If address Is Nothing Then Throw New NullPointerException("address")

			socketConnect(nativefd, address, port)
		End Sub

		Protected Friend Overrides Sub disconnect0(  family As Integer) 'unused
			If fd Is Nothing OrElse (Not fd.valid()) Then Return ' disconnect doesn't throw any exceptions

			socketDisconnect(fdAccess.get(fd))
		End Sub

		Protected Friend Overrides Sub datagramSocketClose()
			If fd Is Nothing OrElse (Not fd.valid()) Then Return ' close doesn't throw any exceptions

			socketClose(fdAccess.get(fd))
			fdAccess.set(fd, -1)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Protected Friend Overrides Sub socketSetOption(  opt As Integer,   val As Object)
			Dim nativefd As Integer = checkAndReturnNativeFD()

			Dim optionValue As Integer = 0

			Select Case opt
				Case IP_TOS , SO_RCVBUF , SO_SNDBUF
					optionValue = CInt(Fix(val))
				Case SO_REUSEADDR
					If exclusiveBind AndAlso localPort <> 0 Then
						' socket already bound, emulate SO_REUSEADDR
						reuseAddressEmulated = True
						isReuseAddress = CBool(val)
						Return
					End If
					'Intentional fallthrough
				Case SO_BROADCAST
					optionValue = If(CBool(val), 1, 0)
				Case Else ' shouldn't get here
					Throw New SocketException("Option not supported")
			End Select

			socketSetIntOption(nativefd, opt, optionValue)
		End Sub

		Protected Friend Overrides Function socketGetOption(  opt As Integer) As Object
			Dim nativefd As Integer = checkAndReturnNativeFD()

			 ' SO_BINDADDR is not a socket option.
			If opt = SO_BINDADDR Then Return socketLocalAddress(nativefd)
			If opt = SO_REUSEADDR AndAlso reuseAddressEmulated Then Return isReuseAddress

			Dim value As Integer = socketGetIntOption(nativefd, opt)
			Dim returnValue As Object = Nothing

			Select Case opt
				Case SO_REUSEADDR , SO_BROADCAST
					returnValue = If(value = 0,  java.lang.[Boolean].FALSE,  java.lang.[Boolean].TRUE)
				Case IP_TOS , SO_RCVBUF , SO_SNDBUF
					returnValue = New Integer?(value)
				Case Else ' shouldn't get here
					Throw New SocketException("Option not supported")
			End Select

			Return returnValue
		End Function

	'     Multicast specific methods.
	'     * Multicasting on a dual layer TCP/IP stack is always done with
	'     * TwoStacksPlainDatagramSocketImpl. This is to overcome the lack
	'     * of behavior defined for multicasting over a dual layer socket by the RFC.
	'     
		Protected Friend Overrides Sub join(  inetaddr As InetAddress,   netIf As NetworkInterface)
			Throw New java.io.IOException("Method not implemented!")
		End Sub

		Protected Friend Overrides Sub leave(  inetaddr As InetAddress,   netIf As NetworkInterface)
			Throw New java.io.IOException("Method not implemented!")
		End Sub

		Protected Friend Overrides Property timeToLive As Integer
			Set(  ttl As Integer)
				Throw New java.io.IOException("Method not implemented!")
			End Set
			Get
				Throw New java.io.IOException("Method not implemented!")
			End Get
		End Property


		<Obsolete> _
		Protected Friend Overrides Property tTL As SByte
			Set(  ttl As SByte)
				Throw New java.io.IOException("Method not implemented!")
			End Set
			Get
				Throw New java.io.IOException("Method not implemented!")
			End Get
		End Property

		' END Multicast specific methods 

		Private Function checkAndReturnNativeFD() As Integer
			If fd Is Nothing OrElse (Not fd.valid()) Then Throw New SocketException("Socket closed")

			Return fdAccess.get(fd)
		End Function

		' Native methods 

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Shared Sub initIDs()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Shared Function socketCreate(  v6Only As Boolean) As Integer
		End Function

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Shared Sub socketBind(  fd As Integer,   localAddress As InetAddress,   localport As Integer,   exclBind As Boolean)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Shared Sub socketConnect(  fd As Integer,   address As InetAddress,   port As Integer)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Shared Sub socketDisconnect(  fd As Integer)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Shared Sub socketClose(  fd As Integer)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Shared Function socketLocalPort(  fd As Integer) As Integer
		End Function

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Shared Function socketLocalAddress(  fd As Integer) As Object
		End Function

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Shared Function socketReceiveOrPeekData(  fd As Integer,   packet As DatagramPacket,   timeout As Integer,   connected As Boolean,   peek As Boolean) As Integer
		End Function

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Shared Sub socketSend(  fd As Integer,   data As SByte(),   offset As Integer,   length As Integer,   address As InetAddress,   port As Integer,   connected As Boolean)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Shared Sub socketSetIntOption(  fd As Integer,   cmd As Integer,   optionValue As Integer)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Shared Function socketGetIntOption(  fd As Integer,   cmd As Integer) As Integer
		End Function
	End Class

End Namespace