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
	''' This class defines the plain DatagramSocketImpl that is used for all
	''' Windows versions lower than Vista. It adds support for IPv6 on
	''' these platforms where available.
	''' 
	''' For backward compatibility windows platforms that do not have IPv6
	''' support also use this implementation, and fd1 gets set to null
	''' during socket creation.
	''' 
	''' @author Chris Hegarty
	''' </summary>

	Friend Class TwoStacksPlainDatagramSocketImpl
		Inherits AbstractPlainDatagramSocketImpl

		' Used for IPv6 on Windows only 
		Private fd1 As java.io.FileDescriptor

	'    
	'     * Needed for ipv6 on windows because we need to know
	'     * if the socket was bound to ::0 or 0.0.0.0, when a caller
	'     * asks for it. In this case, both sockets are used, but we
	'     * don't know whether the caller requested ::0 or 0.0.0.0
	'     * and need to remember it here.
	'     
		Private anyLocalBoundAddr As InetAddress=Nothing

		Private fduse As Integer=-1 ' saved between peek() and receive() calls

	'     saved between successive calls to receive, if data is detected
	'     * on both sockets at same time. To ensure that one socket is not
	'     * starved, they rotate using this field
	'     
		Private lastfd As Integer=-1

		Shared Sub New()
			init()
		End Sub

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

		<MethodImpl(MethodImplOptions.Synchronized)> _
		Protected Friend Overrides Sub create()
			fd1 = New java.io.FileDescriptor
			Try
				MyBase.create()
			Catch e As SocketException
				fd1 = Nothing
				Throw e
			End Try
		End Sub

		<MethodImpl(MethodImplOptions.Synchronized)> _
		Protected Friend Overrides Sub bind(  lport As Integer,   laddr As InetAddress)
			MyBase.bind(lport, laddr)
			If laddr.anyLocalAddress Then anyLocalBoundAddr = laddr
		End Sub

		<MethodImpl(MethodImplOptions.Synchronized)> _
		Protected Friend Overrides Sub bind0(  lport As Integer,   laddr As InetAddress)
			bind0(lport, laddr, exclusiveBind)

		End Sub

		<MethodImpl(MethodImplOptions.Synchronized)> _
		Protected Friend Overrides Sub receive(  p As DatagramPacket)
			Try
				receive0(p)
			Finally
				fduse = -1
			End Try
		End Sub

		Public Overrides Function getOption(  optID As Integer) As Object
			If closed Then Throw New SocketException("Socket Closed")

			If optID = SO_BINDADDR Then
				If (fd IsNot Nothing AndAlso fd1 IsNot Nothing) AndAlso (Not connected) Then Return anyLocalBoundAddr
				Dim family As Integer = If(connectedAddress Is Nothing, -1, connectedAddress.holder().family)
				Return socketLocalAddress(family)
			ElseIf optID = SO_REUSEADDR AndAlso reuseAddressEmulated Then
				Return isReuseAddress
			Else
				Return MyBase.getOption(optID)
			End If
		End Function

		Protected Friend Overrides Sub socketSetOption(  opt As Integer,   val As Object)
			If opt = SO_REUSEADDR AndAlso exclusiveBind AndAlso localPort <> 0 Then
				' socket already bound, emulate
				reuseAddressEmulated = True
				isReuseAddress = CBool(val)
			Else
				socketNativeSetOption(opt, val)
			End If

		End Sub

		Protected Friend  Overrides ReadOnly Property  closed As Boolean
			Get
				Return If(fd Is Nothing AndAlso fd1 Is Nothing, True, False)
			End Get
		End Property

		Protected Friend Overrides Sub close()
			If fd IsNot Nothing OrElse fd1 IsNot Nothing Then
				datagramSocketClose()
				sun.net.ResourceManager.afterUdpClose()
				fd = Nothing
				fd1 = Nothing
			End If
		End Sub

		' Native methods 

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Protected Friend Sub bind0(  lport As Integer,   laddr As InetAddress,   exclBind As Boolean)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Protected Friend Sub send(  p As DatagramPacket)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Protected Friend Function peek(  i As InetAddress) As Integer
		End Function

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Protected Friend Function peekData(  p As DatagramPacket) As Integer
		End Function

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Protected Friend Sub receive0(  p As DatagramPacket)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Protected Friend Sub setTimeToLive(  ttl As Integer)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Protected Friend Function getTimeToLive() As Integer
		End Function

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Protected Friend Sub setTTL(  ttl As SByte)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Protected Friend Function getTTL() As SByte
		End Function

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Protected Friend Sub join(  inetaddr As InetAddress,   netIf As NetworkInterface)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Protected Friend Sub leave(  inetaddr As InetAddress,   netIf As NetworkInterface)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Protected Friend Sub datagramSocketCreate()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Protected Friend Sub datagramSocketClose()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Protected Friend Sub socketNativeSetOption(  opt As Integer,   val As Object)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Protected Friend Function socketGetOption(  opt As Integer) As Object
		End Function

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Protected Friend Sub connect0(  address As InetAddress,   port As Integer)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Protected Friend Function socketLocalAddress(  family As Integer) As Object
		End Function

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Protected Friend Sub disconnect0(  family As Integer)
		End Sub

		''' <summary>
		''' Perform class load-time initializations.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Shared Sub init()
		End Sub
	End Class

End Namespace