Imports Microsoft.VisualBasic
Imports System
Imports System.Runtime.CompilerServices
Imports java.security

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
    ''' Default Socket Implementation. This implementation does
    ''' not implement any security checks.
    ''' Note this class should <b>NOT</b> be public.
    ''' 
    ''' @author  Steven B. Byrne
    ''' </summary>
    Friend MustInherit Class AbstractPlainSocketImpl
        Inherits SocketImpl

        ' instance variable for SO_TIMEOUT 
        Friend timeout As Integer ' timeout in millisec
        ' traffic class
        Private trafficClass As Integer

        Private shut_rd As Boolean = False
        Private shut_wr As Boolean = False

        Private socketInputStream As SocketInputStream = Nothing
        Private socketOutputStream As SocketOutputStream = Nothing

        ' number of threads using the FileDescriptor 
        Protected Friend fdUseCount As Integer = 0

        ' lock when increment/decrementing fdUseCount 
        Protected Friend ReadOnly fdLock As New Object

        ' indicates a close is pending on the file descriptor 
        Protected Friend closePending As Boolean = False

        ' indicates connection reset state 
        Private CONNECTION_NOT_RESET As Integer = 0
        Private CONNECTION_RESET_PENDING As Integer = 1
        Private CONNECTION_RESET As Integer = 2
        Private resetState As Integer
        Private ReadOnly resetLock As New Object

        '    whether this Socket is a stream (TCP) socket or not (UDP)
        '    
        Protected Friend stream As Boolean

        ''' <summary>
        ''' Load net library into runtime.
        ''' </summary>
        Shared Sub New()
            java.security.AccessController.doPrivileged(New PrivilegedActionAnonymousInnerClassHelper(Of T)
        End Sub

        Private Class PrivilegedActionAnonymousInnerClassHelper(Of T)
            Implements PrivilegedAction(Of T)

            Public Overridable Sub run()
                System.loadLibrary("net")
            End Sub
        End Class

        ''' <summary>
        ''' Creates a socket with a boolean that specifies whether this
        ''' is a stream socket (true) or an unconnected UDP socket (false).
        ''' </summary>
        <MethodImpl(MethodImplOptions.Synchronized)>
        Protected Friend Overrides Sub create(ByVal stream As Boolean)
            Me.stream = stream
            If Not stream Then
                sun.net.ResourceManager.beforeUdpCreate()
                ' only create the fd after we know we will be able to create the socket
                fd = New java.io.FileDescriptor
                Try
                    socketCreate(False)
                Catch ioe As java.io.IOException
                    sun.net.ResourceManager.afterUdpClose()
                    fd = Nothing
                    Throw ioe
                End Try
            Else
                fd = New java.io.FileDescriptor
                socketCreate(True)
            End If
            If socket_Renamed IsNot Nothing Then socket_Renamed.createdted()
            If serverSocket_Renamed IsNot Nothing Then serverSocket_Renamed.createdted()
        End Sub

        ''' <summary>
        ''' Creates a socket and connects it to the specified port on
        ''' the specified host. </summary>
        ''' <param name="host"> the specified host </param>
        ''' <param name="port"> the specified port </param>
        Protected Friend Overrides Sub connect(ByVal host As String, ByVal port As Integer)
            Dim connected As Boolean = False
            Try
                Dim address_Renamed As InetAddress = InetAddress.getByName(host)
                Me.port = port
                Me.address = address_Renamed

                connectToAddress(address_Renamed, port, timeout)
                connected = True
            Finally
                If Not connected Then
                    Try
                        close()
                    Catch ioe As java.io.IOException
                        '                     Do nothing. If connect threw an exception then
                        '                       it will be passed up the call stack 
                    End Try
                End If
            End Try
        End Sub

        ''' <summary>
        ''' Creates a socket and connects it to the specified address on
        ''' the specified port. </summary>
        ''' <param name="address"> the address </param>
        ''' <param name="port"> the specified port </param>
        Protected Friend Overrides Sub connect(ByVal address As InetAddress, ByVal port As Integer)
            Me.port = port
            Me.address = address

            Try
                connectToAddress(address, port, timeout)
                Return
            Catch e As java.io.IOException
                ' everything failed
                close()
                Throw e
            End Try
        End Sub

        ''' <summary>
        ''' Creates a socket and connects it to the specified address on
        ''' the specified port. </summary>
        ''' <param name="address"> the address </param>
        ''' <param name="timeout"> the timeout value in milliseconds, or zero for no timeout. </param>
        ''' <exception cref="IOException"> if connection fails </exception>
        ''' <exception cref="IllegalArgumentException"> if address is null or is a
        '''          SocketAddress subclass not supported by this socket
        ''' @since 1.4 </exception>
        Protected Friend Overrides Sub connect(ByVal address As SocketAddress, ByVal timeout As Integer)
            Dim connected As Boolean = False
            Try
                If address Is Nothing OrElse Not (TypeOf address Is InetSocketAddress) Then Throw New IllegalArgumentException("unsupported address type")
                Dim addr As InetSocketAddress = CType(address, InetSocketAddress)
                If addr.unresolved Then Throw New UnknownHostException(addr.hostName)
                Me.port = addr.port
                Me.address = addr.address

                connectToAddress(Me.address, port, timeout)
                connected = True
            Finally
                If Not connected Then
                    Try
                        close()
                    Catch ioe As java.io.IOException
                        '                     Do nothing. If connect threw an exception then
                        '                       it will be passed up the call stack 
                    End Try
                End If
            End Try
        End Sub

        Private Sub connectToAddress(ByVal address As InetAddress, ByVal port As Integer, ByVal timeout As Integer)
            If address.anyLocalAddress Then
                doConnect(inetAddress.localHost, port, timeout)
            Else
                doConnect(address, port, timeout)
            End If
        End Sub

        Public Overrides Sub setOption(ByVal opt As Integer, ByVal val As Object)
            If closedOrPending Then Throw New SocketException("Socket Closed")
            Dim [on] As Boolean = True
            Select Case opt
    '             check type safety b4 going native.  These should never
    '             * fail, since only java.Socket* has access to
    '             * PlainSocketImpl.setOption().
    '             
                Case SO_LINGER
                    If val Is Nothing OrElse (Not (TypeOf val Is Integer?) AndAlso Not (TypeOf val Is Boolean?)) Then Throw New SocketException("Bad parameter for option")
                    If TypeOf val Is Boolean? Then [on] = False
                Case SO_TIMEOUT
                    If val Is Nothing OrElse (Not (TypeOf val Is Integer?)) Then Throw New SocketException("Bad parameter for SO_TIMEOUT")
                    Dim tmp As Integer = CInt(Fix(val))
                    If tmp < 0 Then Throw New IllegalArgumentException("timeout < 0")
                    timeout = tmp
                Case IP_TOS
                    If val Is Nothing OrElse Not (TypeOf val Is Integer?) Then Throw New SocketException("bad argument for IP_TOS")
                    trafficClass = CInt(Fix(val))
                Case SO_BINDADDR
                    Throw New SocketException("Cannot re-bind socket")
                Case TCP_NODELAY
                    If val Is Nothing OrElse Not (TypeOf val Is Boolean?) Then Throw New SocketException("bad parameter for TCP_NODELAY")
                    [on] = CBool(val)
                Case SO_SNDBUF, SO_RCVBUF
                    If val Is Nothing OrElse Not (TypeOf val Is Integer?) OrElse Not (CInt(Fix(val)) > 0) Then Throw New SocketException("bad parameter for SO_SNDBUF " & "or SO_RCVBUF")
                Case SO_KEEPALIVE
                    If val Is Nothing OrElse Not (TypeOf val Is Boolean?) Then Throw New SocketException("bad parameter for SO_KEEPALIVE")
                    [on] = CBool(val)
                Case SO_OOBINLINE
                    If val Is Nothing OrElse Not (TypeOf val Is Boolean?) Then Throw New SocketException("bad parameter for SO_OOBINLINE")
                    [on] = CBool(val)
                Case SO_REUSEADDR
                    If val Is Nothing OrElse Not (TypeOf val Is Boolean?) Then Throw New SocketException("bad parameter for SO_REUSEADDR")
                    [on] = CBool(val)
                Case Else
                    Throw New SocketException("unrecognized TCP option: " & opt)
            End Select
            socketSetOption(opt, [on], val)
        End Sub
        Public Overrides Function getOption(ByVal opt As Integer) As Object
            If closedOrPending Then Throw New SocketException("Socket Closed")
            If opt = SO_TIMEOUT Then Return New Integer?(timeout)
            Dim ret As Integer = 0
            '        
            '         * The native socketGetOption() knows about 3 options.
            '         * The 32 bit value it returns will be interpreted according
            '         * to what we're asking.  A return of -1 means it understands
            '         * the option but its turned off.  It will raise a SocketException
            '         * if "opt" isn't one it understands.
            '         

            Select Case opt
                Case TCP_NODELAY
                    ret = socketGetOption(opt, Nothing)
                    Return Convert.ToBoolean(ret <> -1)
                Case SO_OOBINLINE
                    ret = socketGetOption(opt, Nothing)
                    Return Convert.ToBoolean(ret <> -1)
                Case SO_LINGER
                    ret = socketGetOption(opt, Nothing)
                    Return If(ret = -1,  java.lang.[Boolean].FALSE, CObj(New Integer?(ret)))
                Case SO_REUSEADDR
                    ret = socketGetOption(opt, Nothing)
                    Return Convert.ToBoolean(ret <> -1)
                Case SO_BINDADDR
                    Dim [in] As New InetAddressContainer
                    ret = socketGetOption(opt, [in])
                    Return [in].addr
                Case SO_SNDBUF, SO_RCVBUF
                    ret = socketGetOption(opt, Nothing)
                    Return New Integer?(ret)
                Case IP_TOS
                    Try
                        ret = socketGetOption(opt, Nothing)
                        If ret = -1 Then ' ipv6 tos
                            Return trafficClass
                        Else
                            Return ret
                        End If
                    Catch se As SocketException
                        ' TODO - should make better effort to read TOS or TCLASS
                        Return trafficClass ' ipv6 tos
                    End Try
                Case SO_KEEPALIVE
                    ret = socketGetOption(opt, Nothing)
                    Return Convert.ToBoolean(ret <> -1)
                    ' should never get here
                Case Else
                    Return Nothing
            End Select
        End Function

        ''' <summary>
        ''' The workhorse of the connection operation.  Tries several times to
        ''' establish a connection to the given <host, port>.  If unsuccessful,
        ''' throws an IOException indicating what went wrong.
        ''' </summary>

        SyncLock  Sub  doConnect InetAddress address, Integer port, Integer timeout
			throws java.io.IOException
			SyncLock fdLock
        If (Not closePending) AndAlso (socket_Renamed Is Nothing OrElse (Not socket_Renamed.bound)) Then sun.net.NetHooks.beforeTcpConnect(fd, address, port)
			End SyncLock
        Try
				acquireFD()
				Try
					socketConnect(address, port, timeout)
					' socket may have been closed during poll/select 
					SyncLock fdLock
        If closePending Then Throw New SocketException("Socket closed")
					End SyncLock
        ' If we have a ref. to the Socket, then sets the flags
        ' created, bound & connected to true.
        ' This is normally done in Socket.connect() but some
        ' subclasses of Socket may call impl.connect() directly!
        If socket_Renamed IsNot Nothing Then
						socket_Renamed.boundund()
						socket_Renamed.connectedted()
					End If
        Finally
					releaseFD()
				End Try
        Catch e As java.io.IOException
				close()
				Throw e
        End Try
        End SyncLock

        ''' <summary>
        ''' Binds the socket to the specified address of the specified local port. </summary>
        ''' <param name="address"> the address </param>
        ''' <param name="lport"> the port </param>
        Protected synchronized  Sub  bind(InetAddress address, Integer lport) throws java.io.IOException
		   SyncLock fdLock
        If (Not closePending) AndAlso (socket_Renamed Is Nothing OrElse (Not socket_Renamed.bound)) Then sun.net.NetHooks.beforeTcpBind(fd, address, lport)
		   End SyncLock
			socketBind(address, lport)
			If socket_Renamed IsNot Nothing Then socket_Renamed.boundund()
			If serverSocket_Renamed IsNot Nothing Then serverSocket_Renamed.boundund()

		''' <summary>
		''' Listens, for a specified amount of time, for connections. </summary>
		''' <param name="count"> the amount of time to listen for connections </param>
		Protected synchronized  Sub  listen(Integer count) throws java.io.IOException
			socketListen(count)

		''' <summary>
		''' Accepts connections. </summary>
		''' <param name="s"> the connection </param>
		Protected  Sub  accept(SocketImpl s) throws java.io.IOException
			acquireFD()
			Try
				socketAccept(s)
			Finally
				releaseFD()
			End Try

        ''' <summary>
        ''' Gets an InputStream for this socket.
        ''' </summary>
        Protected synchronized java.io.InputStream inputStream throws java.io.IOException
			SyncLock fdLock
        If closedOrPending Then Throw New java.io.IOException("Socket Closed")
				If shut_rd Then Throw New java.io.IOException("Socket input is shutdown")
				If socketInputStream Is Nothing Then socketInputStream = New SocketInputStream(Me)
			End SyncLock
        Return socketInputStream

		void inputStreameam(socketInputStream in)
			socketInputStream = in

		''' <summary>
		''' Gets an OutputStream for this socket.
		''' </summary>
		Protected synchronized java.io.OutputStream outputStream throws java.io.IOException
			SyncLock fdLock
        If closedOrPending Then Throw New java.io.IOException("Socket Closed")
				If shut_wr Then Throw New java.io.IOException("Socket output is shutdown")
				If socketOutputStream Is Nothing Then socketOutputStream = New SocketOutputStream(Me)
			End SyncLock
        Return socketOutputStream

		void fileDescriptortor(java.io.FileDescriptor fd)
			Me.fd = fd

		void addressess(inetAddress address)
			Me.address = address

		void portort(Integer port)
			Me.port = port

		void localPortort(Integer localport)
			Me.localport = localport

        ''' <summary>
        ''' Returns the number of bytes that can be read without blocking.
        ''' </summary>
        Protected synchronized Integer available() throws java.io.IOException
			If closedOrPending Then Throw New java.io.IOException("Stream closed.")

	'        
	'         * If connection has been reset or shut down for input, then return 0
	'         * to indicate there are no buffered bytes.
	'         
			If connectionReset OrElse shut_rd Then Return 0

	'        
	'         * If no bytes available and we were previously notified
	'         * of a connection reset then we move to the reset state.
	'         *
	'         * If are notified of a connection reset then check
	'         * again if there are bytes buffered on the socket.
	'         
			Dim n As Integer = 0
        Try
				n = socketAvailable()
				If n = 0 AndAlso connectionResetPending Then connectionResetset()
			Catch exc1 As sun.net.ConnectionResetException
				connectionResetPendinging()
				Try
					n = socketAvailable()
					If n = 0 Then connectionResetset()
				Catch exc2 As sun.net.ConnectionResetException
        End Try
        End Try
        Return n

        ''' <summary>
        ''' Closes the socket.
        ''' </summary>
        Protected  Sub  close() throws java.io.IOException
			SyncLock fdLock
        If fd IsNot Nothing Then
        If Not stream Then sun.net.ResourceManager.afterUdpClose()
					If fdUseCount = 0 Then
        If closePending Then Return
						closePending = True
	'                    
	'                     * We close the FileDescriptor in two-steps - first the
	'                     * "pre-close" which closes the socket but doesn't
	'                     * release the underlying file descriptor. This operation
	'                     * may be lengthy due to untransmitted data and a long
	'                     * linger interval. Once the pre-close is done we do the
	'                     * actual socket to release the fd.
	'                     
						Try
							socketPreClose()
						Finally
							socketClose()
						End Try
						fd = Nothing
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
							socketPreClose()
						End If
        End If
        End If
        End SyncLock

		void reset() throws java.io.IOException
			If fd IsNot Nothing Then socketClose()
			fd = Nothing
			MyBase.reset()


        ''' <summary>
        ''' Shutdown read-half of the socket connection;
        ''' </summary>
        Protected  Sub  shutdownInput() throws java.io.IOException
		  If fd IsNot Nothing Then
			  socketShutdown(SHUT_RD)
			  If socketInputStream IsNot Nothing Then socketInputStream.eOF = True
			  shut_rd = True
		  End If

        ''' <summary>
        ''' Shutdown write-half of the socket connection;
        ''' </summary>
        Protected  Sub  shutdownOutput() throws java.io.IOException
		  If fd IsNot Nothing Then
			  socketShutdown(SHUT_WR)
			  shut_wr = True
		  End If

        Protected Boolean supportsUrgentData()
			Return True

        Protected  Sub  sendUrgentData(Integer data) throws java.io.IOException
			If fd Is Nothing Then Throw New java.io.IOException("Socket Closed")
			socketSendUrgentData(data)

		''' <summary>
		''' Cleans up if the user forgets to close it.
		''' </summary>
		Protected  Sub  Finalize() throws java.io.IOException
			close()

	'    
	'     * "Acquires" and returns the FileDescriptor for this impl
	'     *
	'     * A corresponding releaseFD is required to "release" the
	'     * FileDescriptor.
	'     
		java.io.FileDescriptor acquireFD()
			SyncLock fdLock
				fdUseCount += 1
				Return fd
        End SyncLock

	'    
	'     * "Release" the FileDescriptor for this impl.
	'     *
	'     * If the use count goes to -1 then the socket is closed.
	'     
		void releaseFD()
        SyncLock fdLock
				fdUseCount -= 1
				If fdUseCount = -1 Then
        If fd IsNot Nothing Then
        Try
							socketClose()
						Catch e As java.io.IOException
        Finally
							fd = Nothing
						End Try
        End If
        End If
        End SyncLock

        Public Boolean connectionReset
			SyncLock resetLock
        Return (resetState = CONNECTION_RESET)
        End SyncLock

        Public Boolean connectionResetPending
			SyncLock resetLock
        Return (resetState = CONNECTION_RESET_PENDING)
        End SyncLock

        Public  Sub  connectionResetset()
			SyncLock resetLock
				resetState = CONNECTION_RESET
			End SyncLock

        Public  Sub  connectionResetPendinging()
			SyncLock resetLock
        If resetState = CONNECTION_NOT_RESET Then resetState = CONNECTION_RESET_PENDING
			End SyncLock


        '    
        '     * Return true if already closed or close is pending
        '     
        Public Boolean closedOrPending
	'        
	'         * Lock on fdLock to ensure that we wait if a
	'         * close is in progress.
	'         
			SyncLock fdLock
        If closePending OrElse (fd Is Nothing) Then
        Return True
        Else
        Return False
        End If
        End SyncLock

        '    
        '     * Return the current value of SO_TIMEOUT
        '     
        Public Integer timeout
			Return timeout

        '    
        '     * "Pre-close" a socket by dup'ing the file descriptor - this enables
        '     * the socket to be closed without releasing the file descriptor.
        '     
        Private  Sub  socketPreClose() throws java.io.IOException
			socketClose0(True)

	'    
	'     * Close the socket (and release the file descriptor).
	'     
		Protected  Sub  socketClose() throws java.io.IOException
			socketClose0(False)

		abstract  Sub  socketCreate(Boolean isServer) throws java.io.IOException
		abstract  Sub  socketConnect(InetAddress address, Integer port, Integer timeout) throws java.io.IOException
		abstract  Sub  socketBind(InetAddress address, Integer port) throws java.io.IOException
		abstract  Sub  socketListen(Integer count) throws java.io.IOException
		abstract  Sub  socketAccept(SocketImpl s) throws java.io.IOException
		abstract Integer socketAvailable() throws java.io.IOException
		abstract  Sub  socketClose0(Boolean useDeferredClose) throws java.io.IOException
		abstract  Sub  socketShutdown(Integer howto) throws java.io.IOException
		abstract  Sub  socketSetOption(Integer cmd, Boolean On, Object value) throws SocketException
		abstract Integer socketGetOption(Integer opt, Object iaContainerObj) throws SocketException
		abstract  Sub  socketSendUrgentData(Integer data) throws java.io.IOException

		Public final Static Integer SHUT_RD = 0
		Public final Static Integer SHUT_WR = 1
	End Class

End Namespace