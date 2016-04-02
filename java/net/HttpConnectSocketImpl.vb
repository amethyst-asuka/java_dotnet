Imports System
Imports System.Collections.Generic

'
' * Copyright (c) 2010, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' Basic SocketImpl that relies on the internal HTTP protocol handler
	''' implementation to perform the HTTP tunneling and authentication. The
	''' sockets impl is swapped out and replaced with the socket from the HTTP
	''' handler after the tunnel is successfully setup.
	''' 
	''' @since 1.8
	''' </summary>

	'package
	 Friend Class HttpConnectSocketImpl
		 Inherits PlainSocketImpl

		Private Const httpURLClazzStr As String = "sun.net.www.protocol.http.HttpURLConnection"
		Private Const netClientClazzStr As String = "sun.net.NetworkClient"
		Private Const doTunnelingStr As String = "doTunneling"
		Private Shared ReadOnly httpField As Field
		Private Shared ReadOnly serverSocketField As Field
		Private Shared ReadOnly doTunneling_Renamed As Method

		Private ReadOnly server As String
		Private external_address As InetSocketAddress
		Private optionsMap As New Dictionary(Of Integer?, Object)

		Shared Sub New()
			Try
				Dim httpClazz As  [Class] = Type.GetType(httpURLClazzStr, True, Nothing)
				httpField = httpClazz.getDeclaredField("http")
				doTunneling_Renamed = httpClazz.getDeclaredMethod(doTunnelingStr)
				Dim netClientClazz As  [Class] = Type.GetType(netClientClazzStr, True, Nothing)
				serverSocketField = netClientClazz.getDeclaredField("serverSocket")

				java.security.AccessController.doPrivileged(New PrivilegedActionAnonymousInnerClassHelper(Of T)
			Catch x As ReflectiveOperationException
				Throw New InternalError("Should not reach here", x)
			End Try
		End Sub

		Private Class PrivilegedActionAnonymousInnerClassHelper(Of T)
			Implements PrivilegedAction(Of T)

			Public Overridable Function run() As Void
				httpField.accessible = True
				serverSocketField.accessible = True
				Return Nothing
			End Function
		End Class

		Friend Sub New(ByVal server As String, ByVal port As Integer)
			Me.server = server
			Me.port = port
		End Sub

		Friend Sub New(ByVal proxy_Renamed As Proxy)
			Dim a As SocketAddress = proxy_Renamed.address()
			If Not(TypeOf a Is InetSocketAddress) Then Throw New IllegalArgumentException("Unsupported address type")

			Dim ad As InetSocketAddress = CType(a, InetSocketAddress)
			server = ad.hostString
			port = ad.port
		End Sub

		Protected Friend Overrides Sub connect(ByVal endpoint As SocketAddress, ByVal timeout As Integer)
			If endpoint Is Nothing OrElse Not(TypeOf endpoint Is InetSocketAddress) Then Throw New IllegalArgumentException("Unsupported address type")
			Dim epoint As InetSocketAddress = CType(endpoint, InetSocketAddress)
			Dim destHost As String = If(epoint.unresolved, epoint.hostName, epoint.address.hostAddress)
			Dim destPort As Integer = epoint.port

			Dim security As SecurityManager = System.securityManager
			If security IsNot Nothing Then security.checkConnect(destHost, destPort)

			' Connect to the HTTP proxy server
			Dim urlString As String = "http://" & destHost & ":" & destPort
			Dim httpSocket As Socket = privilegedDoTunnel(urlString, timeout)

			' Success!
			external_address = epoint

			' close the original socket impl and release its descriptor
			close()

			' update the Sockets impl to the impl from the http Socket
			Dim psi As AbstractPlainSocketImpl = CType(httpSocket.impl, AbstractPlainSocketImpl)
			Me.socket.impl = psi

			' best effort is made to try and reset options previously set
'JAVA TO VB CONVERTER TODO TASK: There is no .NET Dictionary equivalent to the Java 'entrySet' method:
			Dim options As java.util.Set(Of KeyValuePair(Of Integer?, Object)) = optionsMap.entrySet()
			Try
				For Each entry As KeyValuePair(Of Integer?, Object) In options
					psi.optionion(entry.Key, entry.Value)
				Next entry ' gulp!
			Catch x As java.io.IOException
			End Try
		End Sub

		Public Overrides Sub setOption(ByVal opt As Integer, ByVal val As Object)
			MyBase.optionion(opt, val)

			If external_address IsNot Nothing Then Return ' we're connected, just return

			' store options so that they can be re-applied to the impl after connect
			optionsMap(opt) = val
		End Sub

		Private Function privilegedDoTunnel(ByVal urlString As String, ByVal timeout As Integer) As Socket
			Try
				Return java.security.AccessController.doPrivileged(New PrivilegedExceptionActionAnonymousInnerClassHelper(Of T)
			Catch pae As java.security.PrivilegedActionException
				Throw CType(pae.exception, java.io.IOException)
			End Try
		End Function

		Private Class PrivilegedExceptionActionAnonymousInnerClassHelper(Of T)
			Implements PrivilegedExceptionAction(Of T)

			Public Overridable Function run() As Socket
				Return outerInstance.doTunnel(urlString, outerInstance.timeout)
			End Function
		End Class

		Private Function doTunnel(ByVal urlString As String, ByVal connectTimeout As Integer) As Socket
			Dim proxy_Renamed As New Proxy(Proxy.Type.HTTP, New InetSocketAddress(server, port))
			Dim destURL As New URL(urlString)
			Dim conn As HttpURLConnection = CType(destURL.openConnection(proxy_Renamed), HttpURLConnection)
			conn.connectTimeout = connectTimeout
			conn.readTimeout = Me.timeout
			conn.connect()
			doTunneling(conn)
			Try
				Dim httpClient As Object = httpField.get(conn)
				Return CType(serverSocketField.get(httpClient), Socket)
			Catch x As IllegalAccessException
				Throw New InternalError("Should not reach here", x)
			End Try
		End Function

		Private Sub doTunneling(ByVal conn As HttpURLConnection)
			Try
				doTunneling_Renamed.invoke(conn)
			Catch x As ReflectiveOperationException
				Throw New InternalError("Should not reach here", x)
			End Try
		End Sub

		Protected Friend  Overrides ReadOnly Property  inetAddress As InetAddress
			Get
				If external_address IsNot Nothing Then
					Return external_address.address
				Else
					Return MyBase.inetAddress
				End If
			End Get
		End Property

		Protected Friend  Overrides ReadOnly Property  port As Integer
			Get
				If external_address IsNot Nothing Then
					Return external_address.port
				Else
					Return MyBase.port
				End If
			End Get
		End Property

		Protected Friend  Overrides ReadOnly Property  localPort As Integer
			Get
				If socket_Renamed IsNot Nothing Then Return MyBase.localPort
				If external_address IsNot Nothing Then
					Return external_address.port
				Else
					Return MyBase.localPort
				End If
			End Get
		End Property
	 End Class

End Namespace