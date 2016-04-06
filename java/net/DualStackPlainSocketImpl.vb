Imports Microsoft.VisualBasic
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
	''' This class defines the plain SocketImpl that is used on Windows platforms
	''' greater or equal to Windows Vista. These platforms have a dual
	''' layer TCP/IP stack and can handle both IPv4 and IPV6 through a
	''' single file descriptor.
	''' 
	''' @author Chris Hegarty
	''' </summary>

	Friend Class DualStackPlainSocketImpl
		Inherits AbstractPlainSocketImpl

		Friend Shared fdAccess As sun.misc.JavaIOFileDescriptorAccess = sun.misc.SharedSecrets.javaIOFileDescriptorAccess


		' true if this socket is exclusively bound
		Private ReadOnly exclusiveBind As Boolean

		' emulates SO_REUSEADDR when exclusiveBind is true
		Private isReuseAddress As Boolean

		Public Sub New(  exclBind As Boolean)
			exclusiveBind = exclBind
		End Sub

		Public Sub New(  fd As java.io.FileDescriptor,   exclBind As Boolean)
			Me.fd = fd
			exclusiveBind = exclBind
		End Sub

		Friend Overrides Sub socketCreate(  stream As Boolean)
			If fd Is Nothing Then Throw New SocketException("Socket closed")

			Dim newfd As Integer = socket0(stream, False) 'v6 Only

			fdAccess.set(fd, newfd)
		End Sub

		Friend Overrides Sub socketConnect(  address As InetAddress,   port As Integer,   timeout As Integer)
			Dim nativefd As Integer = checkAndReturnNativeFD()

			If address Is Nothing Then Throw New NullPointerException("inet address argument is null.")

			Dim connectResult As Integer
			If timeout <= 0 Then
				connectResult = connect0(nativefd, address, port)
			Else
				configureBlocking(nativefd, False)
				Try
					connectResult = connect0(nativefd, address, port)
					If connectResult = WOULDBLOCK Then waitForConnect(nativefd, timeout)
				Finally
					configureBlocking(nativefd, True)
				End Try
			End If
	'        
	'         * We need to set the local port field. If bind was called
	'         * previous to the connect (by the client) then localport field
	'         * will already be set.
	'         
			If localport = 0 Then localport = localPort0(nativefd)
		End Sub

		Friend Overrides Sub socketBind(  address As InetAddress,   port As Integer)
			Dim nativefd As Integer = checkAndReturnNativeFD()

			If address Is Nothing Then Throw New NullPointerException("inet address argument is null.")

			bind0(nativefd, address, port, exclusiveBind)
			If port = 0 Then
				localport = localPort0(nativefd)
			Else
				localport = port
			End If

			Me.address = address
		End Sub

		Friend Overrides Sub socketListen(  backlog As Integer)
			Dim nativefd As Integer = checkAndReturnNativeFD()

			listen0(nativefd, backlog)
		End Sub

		Friend Overrides Sub socketAccept(  s As SocketImpl)
			Dim nativefd As Integer = checkAndReturnNativeFD()

			If s Is Nothing Then Throw New NullPointerException("socket is null")

			Dim newfd As Integer = -1
			Dim isaa As InetSocketAddress() = New InetSocketAddress(0){}
			If timeout <= 0 Then
				newfd = accept0(nativefd, isaa)
			Else
				configureBlocking(nativefd, False)
				Try
					waitForNewConnection(nativefd, timeout)
					newfd = accept0(nativefd, isaa)
					If newfd <> -1 Then configureBlocking(newfd, True)
				Finally
					configureBlocking(nativefd, True)
				End Try
			End If
			' Update (SocketImpl)s' fd 
			fdAccess.set(s.fd, newfd)
			' Update socketImpls remote port, address and localport 
			Dim isa As InetSocketAddress = isaa(0)
			s.port = isa.port
			s.address = isa.address
			s.localport = localport
		End Sub

		Friend Overrides Function socketAvailable() As Integer
			Dim nativefd As Integer = checkAndReturnNativeFD()
			Return available0(nativefd)
		End Function

		Friend Overrides Sub socketClose0(  useDeferredClose As Boolean) 'unused
			If fd Is Nothing Then Throw New SocketException("Socket closed")

			If Not fd.valid() Then Return

			Dim nativefd As Integer = fdAccess.get(fd)
			fdAccess.set(fd, -1)
			close0(nativefd)
		End Sub

		Friend Overrides Sub socketShutdown(  howto As Integer)
			Dim nativefd As Integer = checkAndReturnNativeFD()
			shutdown0(nativefd, howto)
		End Sub

		' Intentional fallthrough after SO_REUSEADDR
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Friend Overrides Sub socketSetOption(  opt As Integer,   [on] As Boolean,   value As Object)
			Dim nativefd As Integer = checkAndReturnNativeFD()

			If opt = SO_TIMEOUT Then ' timeout implemented through select. Return

			Dim optionValue As Integer = 0

			Select Case opt
				Case SO_REUSEADDR
					If exclusiveBind Then
						' SO_REUSEADDR emulated when using exclusive bind
						isReuseAddress = [on]
						Return
					End If
					' intentional fallthrough
				Case TCP_NODELAY , SO_OOBINLINE , SO_KEEPALIVE
					optionValue = If([on], 1, 0)
				Case SO_SNDBUF , SO_RCVBUF , IP_TOS
					optionValue = CInt(Fix(value))
				Case SO_LINGER
					If [on] Then
						optionValue = CInt(Fix(value))
					Else
						optionValue = -1
					End If
				Case Else ' shouldn't get here
					Throw New SocketException("Option not supported")
			End Select

			intOptionion(nativefd, opt, optionValue)
		End Sub

		Friend Overrides Function socketGetOption(  opt As Integer,   iaContainerObj As Object) As Integer
			Dim nativefd As Integer = checkAndReturnNativeFD()

			' SO_BINDADDR is not a socket option.
			If opt = SO_BINDADDR Then
				localAddress(nativefd, CType(iaContainerObj, InetAddressContainer))
				Return 0 ' return value doesn't matter.
			End If

			' SO_REUSEADDR emulated when using exclusive bind
			If opt = SO_REUSEADDR AndAlso exclusiveBind Then Return If(isReuseAddress, 1, -1)

			Dim value As Integer = getIntOption(nativefd, opt)

			Select Case opt
				Case TCP_NODELAY , SO_OOBINLINE , SO_KEEPALIVE , SO_REUSEADDR
					Return If(value = 0, -1, 1)
			End Select
			Return value
		End Function

		Friend Overrides Sub socketSendUrgentData(  data As Integer)
			Dim nativefd As Integer = checkAndReturnNativeFD()
			sendOOB(nativefd, data)
		End Sub

		Private Function checkAndReturnNativeFD() As Integer
			If fd Is Nothing OrElse (Not fd.valid()) Then Throw New SocketException("Socket closed")

			Return fdAccess.get(fd)
		End Function

		Friend Const WOULDBLOCK As Integer = -2 ' Nothing available (non-blocking)

		Shared Sub New()
			initIDs()
		End Sub

		' Native methods 

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Friend Shared Sub initIDs()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Friend Shared Function socket0(  stream As Boolean,   v6Only As Boolean) As Integer
		End Function

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Friend Shared Sub bind0(  fd As Integer,   localAddress As InetAddress,   localport As Integer,   exclBind As Boolean)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Friend Shared Function connect0(  fd As Integer,   remote As InetAddress,   remotePort As Integer) As Integer
		End Function

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Friend Shared Sub waitForConnect(  fd As Integer,   timeout As Integer)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Friend Shared Function localPort0(  fd As Integer) As Integer
		End Function

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Friend Shared Sub localAddress(  fd As Integer,   [in] As InetAddressContainer)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Friend Shared Sub listen0(  fd As Integer,   backlog As Integer)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Friend Shared Function accept0(  fd As Integer,   isaa As InetSocketAddress()) As Integer
		End Function

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Friend Shared Sub waitForNewConnection(  fd As Integer,   timeout As Integer)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Friend Shared Function available0(  fd As Integer) As Integer
		End Function

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Friend Shared Sub close0(  fd As Integer)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Friend Shared Sub shutdown0(  fd As Integer,   howto As Integer)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Friend Shared Sub setIntOption(  fd As Integer,   cmd As Integer,   optionValue As Integer)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Friend Shared Function getIntOption(  fd As Integer,   cmd As Integer) As Integer
		End Function

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Friend Shared Sub sendOOB(  fd As Integer,   data As Integer)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Friend Shared Sub configureBlocking(  fd As Integer,   blocking As Boolean)
		End Sub
	End Class

End Namespace