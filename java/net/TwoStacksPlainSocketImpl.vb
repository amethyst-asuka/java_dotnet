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


	'
	' * This class defines the plain SocketImpl that is used for all
	' * Windows version lower than Vista. It adds support for IPv6 on
	' * these platforms where available.
	' *
	' * For backward compatibility Windows platforms that do not have IPv6
	' * support also use this implementation, and fd1 gets set to null
	' * during socket creation.
	' *
	' * @author Chris Hegarty
	' 

	Friend Class TwoStacksPlainSocketImpl
		Inherits AbstractPlainSocketImpl

	'     second fd, used for ipv6 on windows only.
	'     * fd1 is used for listeners and for client sockets at initialization
	'     * until the socket is connected. Up to this point fd always refers
	'     * to the ipv4 socket and fd1 to the ipv6 socket. After the socket
	'     * becomes connected, fd always refers to the connected socket
	'     * (either v4 or v6) and fd1 is closed.
	'     *
	'     * For ServerSockets, fd always refers to the v4 listener and
	'     * fd1 the v6 listener.
	'     
		Private fd1 As java.io.FileDescriptor

	'    
	'     * Needed for ipv6 on windows because we need to know
	'     * if the socket is bound to ::0 or 0.0.0.0, when a caller
	'     * asks for it. Otherwise we don't know which socket to ask.
	'     
		Private anyLocalBoundAddr As InetAddress = Nothing

	'     to prevent starvation when listening on two sockets, this is
	'     * is used to hold the id of the last socket we accepted on.
	'     
		Private lastfd As Integer = -1

		' true if this socket is exclusively bound
		Private ReadOnly exclusiveBind As Boolean

		' emulates SO_REUSEADDR when exclusiveBind is true
		Private isReuseAddress As Boolean

		Shared Sub New()
			initProto()
		End Sub

		Public Sub New(ByVal exclBind As Boolean)
			exclusiveBind = exclBind
		End Sub

		Public Sub New(ByVal fd As java.io.FileDescriptor, ByVal exclBind As Boolean)
			Me.fd = fd
			exclusiveBind = exclBind
		End Sub

		''' <summary>
		''' Creates a socket with a boolean that specifies whether this
		''' is a stream socket (true) or an unconnected UDP socket (false).
		''' </summary>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Protected Friend Overrides Sub create(ByVal stream As Boolean)
			fd1 = New java.io.FileDescriptor
			Try
				MyBase.create(stream)
			Catch e As java.io.IOException
				fd1 = Nothing
				Throw e
			End Try
		End Sub

		 ''' <summary>
		 ''' Binds the socket to the specified address of the specified local port. </summary>
		 ''' <param name="address"> the address </param>
		 ''' <param name="port"> the port </param>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Protected Friend Overrides Sub bind(ByVal address As InetAddress, ByVal lport As Integer)
			MyBase.bind(address, lport)
			If address.anyLocalAddress Then anyLocalBoundAddr = address
		End Sub

		Public Overrides Function getOption(ByVal opt As Integer) As Object
			If closedOrPending Then Throw New SocketException("Socket Closed")
			If opt = SO_BINDADDR Then
				If fd IsNot Nothing AndAlso fd1 IsNot Nothing Then Return anyLocalBoundAddr
				Dim [in] As New InetAddressContainer
				socketGetOption(opt, [in])
				Return [in].addr
			ElseIf opt = SO_REUSEADDR AndAlso exclusiveBind Then
				' SO_REUSEADDR emulated when using exclusive bind
				Return isReuseAddress
			Else
				Return MyBase.getOption(opt)
			End If
		End Function

		Friend Overrides Sub socketBind(ByVal address As InetAddress, ByVal port As Integer)
			socketBind(address, port, exclusiveBind)
		End Sub

		Friend Overrides Sub socketSetOption(ByVal opt As Integer, ByVal [on] As Boolean, ByVal value As Object)
			' SO_REUSEADDR emulated when using exclusive bind
			If opt = SO_REUSEADDR AndAlso exclusiveBind Then
				isReuseAddress = [on]
			Else
				socketNativeSetOption(opt, [on], value)
			End If
		End Sub

		''' <summary>
		''' Closes the socket.
		''' </summary>
		Protected Friend Overrides Sub close()
			SyncLock fdLock
				If fd IsNot Nothing OrElse fd1 IsNot Nothing Then
					If Not stream Then sun.net.ResourceManager.afterUdpClose()
					If fdUseCount = 0 Then
						If closePending Then Return
						closePending = True
						socketClose()
						fd = Nothing
						fd1 = Nothing
						Return
					Else
	'                    
	'                     * If a thread has acquired the fd and a close
	'                     * isn't pending then use a deferred close.
	'                     * Also decrement fdUseCount to signal the last
	'                     * thread that releases the fd to close it.
	'                     
						If Not closePending Then
							closePending = True
							fdUseCount -= 1
							socketClose()
						End If
					End If
				End If
			End SyncLock
		End Sub

		Friend Overrides Sub reset()
			If fd IsNot Nothing OrElse fd1 IsNot Nothing Then socketClose()
			fd = Nothing
			fd1 = Nothing
			MyBase.reset()
		End Sub

	'    
	'     * Return true if already closed or close is pending
	'     
		Public Property Overrides closedOrPending As Boolean
			Get
		'        
		'         * Lock on fdLock to ensure that we wait if a
		'         * close is in progress.
		'         
				SyncLock fdLock
					If closePending OrElse (fd Is Nothing AndAlso fd1 Is Nothing) Then
						Return True
					Else
						Return False
					End If
				End SyncLock
			End Get
		End Property

		' Native methods 

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Friend Shared Sub initProto()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Friend Sub socketCreate(ByVal isServer As Boolean)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Friend Sub socketConnect(ByVal address As InetAddress, ByVal port As Integer, ByVal timeout As Integer)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Friend Sub socketBind(ByVal address As InetAddress, ByVal port As Integer, ByVal exclBind As Boolean)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Friend Sub socketListen(ByVal count As Integer)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Friend Sub socketAccept(ByVal s As SocketImpl)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Friend Function socketAvailable() As Integer
		End Function

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Friend Sub socketClose0(ByVal useDeferredClose As Boolean)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Friend Sub socketShutdown(ByVal howto As Integer)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Friend Sub socketNativeSetOption(ByVal cmd As Integer, ByVal [on] As Boolean, ByVal value As Object)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Friend Function socketGetOption(ByVal opt As Integer, ByVal iaContainerObj As Object) As Integer
		End Function

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Friend Sub socketSendUrgentData(ByVal data As Integer)
		End Sub
	End Class

End Namespace