Imports System
Imports System.Runtime.CompilerServices
Imports System.Diagnostics
Imports System.Collections.Generic

'
' * Copyright (c) 2000, 2013, Oracle and/or its affiliates. All rights reserved.
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
	' import org.ietf.jgss.*; 

	''' <summary>
	''' SOCKS (V4 & V5) TCP socket implementation (RFC 1928).
	''' This is a subclass of PlainSocketImpl.
	''' Note this class should <b>NOT</b> be public.
	''' </summary>

	Friend Class SocksSocketImpl
		Inherits PlainSocketImpl
		Implements SocksConsts

		Private server As String = Nothing
		Private serverPort As Integer = DEFAULT_PORT
		Private external_address As InetSocketAddress
		Private useV4 As Boolean = False
		Private cmdsock As Socket = Nothing
		Private cmdIn As java.io.InputStream = Nothing
		Private cmdOut As java.io.OutputStream = Nothing
		' true if the Proxy has been set programatically 
		Private applicationSetProxy As Boolean ' false


		Friend Sub New()
			' Nothing needed
		End Sub

		Friend Sub New(ByVal server As String, ByVal port As Integer)
			Me.server = server
			Me.serverPort = (If(port = -1, DEFAULT_PORT, port))
		End Sub

		Friend Sub New(ByVal proxy_Renamed As Proxy)
			Dim a As SocketAddress = proxy_Renamed.address()
			If TypeOf a Is InetSocketAddress Then
				Dim ad As InetSocketAddress = CType(a, InetSocketAddress)
				' Use getHostString() to avoid reverse lookups
				server = ad.hostString
				serverPort = ad.port
			End If
		End Sub

		Friend Overridable Sub setV4()
			useV4 = True
		End Sub

		<MethodImpl(MethodImplOptions.Synchronized)> _
		Private Sub privilegedConnect(ByVal host As String, ByVal port As Integer, ByVal timeout As Integer)
			Try
				java.security.AccessController.doPrivileged(New PrivilegedExceptionActionAnonymousInnerClassHelper(Of T)
			Catch pae As java.security.PrivilegedActionException
				Throw CType(pae.exception, java.io.IOException)
			End Try
		End Sub

		Private Class PrivilegedExceptionActionAnonymousInnerClassHelper(Of T)
			Implements java.security.PrivilegedExceptionAction(Of T)

			Public Overridable Function run() As Void
					  outerInstance.superConnectServer(host, outerInstance.port, outerInstance.timeout)
					  outerInstance.cmdIn = outerInstance.inputStream
					  outerInstance.cmdOut = outerInstance.outputStream
					  Return Nothing
			End Function
		End Class

		Private Sub superConnectServer(ByVal host As String, ByVal port As Integer, ByVal timeout As Integer)
			MyBase.connect(New InetSocketAddress(host, port), timeout)
		End Sub

		Private Shared Function remainingMillis(ByVal deadlineMillis As Long) As Integer
			If deadlineMillis = 0L Then Return 0

			Dim remaining As Long = deadlineMillis - System.currentTimeMillis()
			If remaining > 0 Then Return CInt(remaining)

			Throw New SocketTimeoutException
		End Function

		Private Function readSocksReply(ByVal [in] As java.io.InputStream, ByVal data As SByte()) As Integer
			Return readSocksReply([in], data, 0L)
		End Function

		Private Function readSocksReply(ByVal [in] As java.io.InputStream, ByVal data As SByte(), ByVal deadlineMillis As Long) As Integer
			Dim len As Integer = data.Length
			Dim received As Integer = 0
			Dim attempts As Integer = 0
			Do While received < len AndAlso attempts < 3
				Dim count As Integer
				Try
					count = CType([in], SocketInputStream).read(data, received, len - received, remainingMillis(deadlineMillis))
				Catch e As SocketTimeoutException
					Throw New SocketTimeoutException("Connect timed out")
				End Try
				If count < 0 Then Throw New SocketException("Malformed reply from SOCKS server")
				received += count
				attempts += 1
			Loop
			Return received
		End Function

		''' <summary>
		''' Provides the authentication machanism required by the proxy.
		''' </summary>
		Private Function authenticate(ByVal method As SByte, ByVal [in] As java.io.InputStream, ByVal out As java.io.BufferedOutputStream) As Boolean
			Return authenticate(method, [in], out, 0L)
		End Function

		Private Function authenticate(ByVal method As SByte, ByVal [in] As java.io.InputStream, ByVal out As java.io.BufferedOutputStream, ByVal deadlineMillis As Long) As Boolean
			' No Authentication required. We're done then!
			If method = NO_AUTH Then Return True
			''' <summary>
			''' User/Password authentication. Try, in that order :
			''' - The application provided Authenticator, if any
			''' - the user.name & no password (backward compatibility behavior).
			''' </summary>
			If method = USER_PASSW Then
				Dim userName_Renamed As String
				Dim password As String = Nothing
				Dim addr As InetAddress = InetAddress.getByName(server)
				Dim pw As PasswordAuthentication = java.security.AccessController.doPrivileged(New PrivilegedActionAnonymousInnerClassHelper(Of T)
				If pw IsNot Nothing Then
					userName_Renamed = pw.userName
					password = New String(pw.password)
				Else
					userName_Renamed = java.security.AccessController.doPrivileged(New sun.security.action.GetPropertyAction("user.name"))
				End If
				If userName_Renamed Is Nothing Then Return False
				out.write(1)
				out.write(userName_Renamed.length())
				Try
					out.write(userName_Renamed.getBytes("ISO-8859-1"))
				Catch uee As java.io.UnsupportedEncodingException
					Debug.Assert(False)
				End Try
				If password IsNot Nothing Then
					out.write(password.length())
					Try
						out.write(password.getBytes("ISO-8859-1"))
					Catch uee As java.io.UnsupportedEncodingException
						Debug.Assert(False)
					End Try
				Else
					out.write(0)
				End If
				out.flush()
				Dim data As SByte() = New SByte(1){}
				Dim i As Integer = readSocksReply([in], data, deadlineMillis)
				If i <> 2 OrElse data(1) <> 0 Then
	'                 RFC 1929 specifies that the connection MUST be closed if
	'                   authentication fails 
					out.close()
					[in].close()
					Return False
				End If
				' Authentication succeeded 
				Return True
			End If
			''' <summary>
			''' GSSAPI authentication mechanism.
			''' Unfortunately the RFC seems out of sync with the Reference
			''' implementation. I'll leave this in for future completion.
			''' </summary>
	'      if (method == GSSAPI) {
	'          try {
	'              GSSManager manager = GSSManager.getInstance();
	'              GSSName name = manager.createName("SERVICE:socks@"+server,
	'                                                   null);
	'              GSSContext context = manager.createContext(name, null, null,
	'                                                         GSSContext.DEFAULT_LIFETIME);
	'              context.requestMutualAuth(true);
	'              context.requestReplayDet(true);
	'              context.requestSequenceDet(true);
	'              context.requestCredDeleg(true);
	'              byte []inToken = new byte[0];
	'              while (!context.isEstablished()) {
	'                  byte[] outToken
	'                      = context.initSecContext(inToken, 0, inToken.length);
	'                  // send the output token if generated
	'                  if (outToken != null) {
	'                      out.write(1);
	'                      out.write(1);
	'                      out.writeShort(outToken.length);
	'                      out.write(outToken);
	'                      out.flush();
	'                      data = new byte[2];
	'                      i = readSocksReply(in, data, deadlineMillis);
	'                      if (i != 2 || data[1] == 0xff) {
	'                          in.close();
	'                          out.close();
	'                          return false;
	'                      }
	'                      i = readSocksReply(in, data, deadlineMillis);
	'                      int len = 0;
	'                      len = ((int)data[0] & 0xff) << 8;
	'                      len += data[1];
	'                      data = new byte[len];
	'                      i = readSocksReply(in, data, deadlineMillis);
	'                      if (i == len)
	'                          return true;
	'                      in.close();
	'                      out.close();
	'                  }
	'              }
	'          } catch (GSSException e) {
	'              /* RFC 1961 states that if Context initialisation fails the connection
	'                 MUST be closed */
	'              e.printStackTrace();
	'              in.close();
	'              out.close();
	'          }
	'      }
			Return False
		End Function

		Private Class PrivilegedActionAnonymousInnerClassHelper(Of T)
			Implements java.security.PrivilegedAction(Of T)

			Public Overridable Function run() As PasswordAuthentication
					Return Authenticator.requestPasswordAuthentication(outerInstance.server, addr, outerInstance.serverPort, "SOCKS5", "SOCKS authentication", Nothing)
			End Function
		End Class

		Private Sub connectV4(ByVal [in] As java.io.InputStream, ByVal out As java.io.OutputStream, ByVal endpoint As InetSocketAddress, ByVal deadlineMillis As Long)
			If Not(TypeOf endpoint.address Is Inet4Address) Then Throw New SocketException("SOCKS V4 requires IPv4 only addresses")
			out.write(PROTO_VERS4)
			out.write(CONNECT)
			out.write((endpoint.port >> 8) And &Hff)
			out.write((endpoint.port >> 0) And &Hff)
			out.write(endpoint.address.address)
			Dim userName_Renamed As String = userName
			Try
				out.write(userName_Renamed.getBytes("ISO-8859-1"))
			Catch uee As java.io.UnsupportedEncodingException
				Debug.Assert(False)
			End Try
			out.write(0)
			out.flush()
			Dim data As SByte() = New SByte(7){}
			Dim n As Integer = readSocksReply([in], data, deadlineMillis)
			If n <> 8 Then Throw New SocketException("Reply from SOCKS server has bad length: " & n)
			If data(0) <> 0 AndAlso data(0) <> 4 Then Throw New SocketException("Reply from SOCKS server has bad version")
			Dim ex As SocketException = Nothing
			Select Case data(1)
			Case 90
				' Success!
				external_address = endpoint
			Case 91
				ex = New SocketException("SOCKS request rejected")
			Case 92
				ex = New SocketException("SOCKS server couldn't reach destination")
			Case 93
				ex = New SocketException("SOCKS authentication failed")
			Case Else
				ex = New SocketException("Reply from SOCKS server contains bad status")
			End Select
			If ex IsNot Nothing Then
				[in].close()
				out.close()
				Throw ex
			End If
		End Sub

		''' <summary>
		''' Connects the Socks Socket to the specified endpoint. It will first
		''' connect to the SOCKS proxy and negotiate the access. If the proxy
		''' grants the connections, then the connect is successful and all
		''' further traffic will go to the "real" endpoint.
		''' </summary>
		''' <param name="endpoint">        the {@code SocketAddress} to connect to. </param>
		''' <param name="timeout">         the timeout value in milliseconds </param>
		''' <exception cref="IOException">     if the connection can't be established. </exception>
		''' <exception cref="SecurityException"> if there is a security manager and it
		'''                          doesn't allow the connection </exception>
		''' <exception cref="IllegalArgumentException"> if endpoint is null or a
		'''          SocketAddress subclass not supported by this socket </exception>
		Protected Friend Overrides Sub connect(ByVal endpoint As SocketAddress, ByVal timeout As Integer)
			Dim deadlineMillis As Long

			If timeout = 0 Then
				deadlineMillis = 0L
			Else
				Dim finish As Long = System.currentTimeMillis() + timeout
				deadlineMillis = If(finish < 0, java.lang.[Long].Max_Value, finish)
			End If

			Dim security As SecurityManager = System.securityManager
			If endpoint Is Nothing OrElse Not(TypeOf endpoint Is InetSocketAddress) Then Throw New IllegalArgumentException("Unsupported address type")
			Dim epoint As InetSocketAddress = CType(endpoint, InetSocketAddress)
			If security IsNot Nothing Then
				If epoint.unresolved Then
					security.checkConnect(epoint.hostName, epoint.port)
				Else
					security.checkConnect(epoint.address.hostAddress, epoint.port)
				End If
			End If
			If server Is Nothing Then
				' This is the general case
				' server is not null only when the socket was created with a
				' specified proxy in which case it does bypass the ProxySelector
				Dim sel As ProxySelector = java.security.AccessController.doPrivileged(New PrivilegedActionAnonymousInnerClassHelper2(Of T)
				If sel Is Nothing Then
	'                
	'                 * No default proxySelector --> direct connection
	'                 
					MyBase.connect(epoint, remainingMillis(deadlineMillis))
					Return
				End If
				Dim uri As URI
				' Use getHostString() to avoid reverse lookups
				Dim host As String = epoint.hostString
				' IPv6 litteral?
				If TypeOf epoint.address Is Inet6Address AndAlso ((Not host.StartsWith("["))) AndAlso (host.IndexOf(":") >= 0) Then host = "[" & host & "]"
				Try
					uri = New URI("socket://" & sun.net.www.ParseUtil.encodePath(host) & ":" & epoint.port)
				Catch e As URISyntaxException
					' This shouldn't happen
					Debug.Assert(False, e)
					uri = Nothing
				End Try
				Dim p As Proxy = Nothing
				Dim savedExc As java.io.IOException = Nothing
				Dim iProxy As IEnumerator(Of Proxy) = Nothing
				iProxy = sel.select(uri).GetEnumerator()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				If iProxy Is Nothing OrElse Not(iProxy.hasNext()) Then
					MyBase.connect(epoint, remainingMillis(deadlineMillis))
					Return
				End If
				Do While iProxy.MoveNext()
					p = iProxy.Current
					If p Is Nothing OrElse p.type() <> Proxy.Type.SOCKS Then
						MyBase.connect(epoint, remainingMillis(deadlineMillis))
						Return
					End If

					If Not(TypeOf p.address() Is InetSocketAddress) Then Throw New SocketException("Unknown address type for proxy: " & p)
					' Use getHostString() to avoid reverse lookups
					server = CType(p.address(), InetSocketAddress).hostString
					serverPort = CType(p.address(), InetSocketAddress).port
					If TypeOf p Is sun.net.SocksProxy Then
						If CType(p, sun.net.SocksProxy).protocolVersion() = 4 Then useV4 = True
					End If

					' Connects to the SOCKS server
					Try
						privilegedConnect(server, serverPort, remainingMillis(deadlineMillis))
						' Worked, let's get outta here
						Exit Do
					Catch e As java.io.IOException
						' Ooops, let's notify the ProxySelector
						sel.connectFailed(uri,p.address(),e)
						server = Nothing
						serverPort = -1
						savedExc = e
						' Will continue the while loop and try the next proxy
					End Try
				Loop

	'            
	'             * If server is still null at this point, none of the proxy
	'             * worked
	'             
				If server Is Nothing Then Throw New SocketException("Can't connect to SOCKS proxy:" & savedExc.Message)
			Else
				' Connects to the SOCKS server
				Try
					privilegedConnect(server, serverPort, remainingMillis(deadlineMillis))
				Catch e As java.io.IOException
					Throw New SocketException(e.Message)
				End Try
			End If

			' cmdIn & cmdOut were initialized during the privilegedConnect() call
			Dim out As New java.io.BufferedOutputStream(cmdOut, 512)
			Dim [in] As java.io.InputStream = cmdIn

			If useV4 Then
				' SOCKS Protocol version 4 doesn't know how to deal with
				' DOMAIN type of addresses (unresolved addresses here)
				If epoint.unresolved Then Throw New UnknownHostException(epoint.ToString())
				connectV4([in], out, epoint, deadlineMillis)
				Return
			End If

			' This is SOCKS V5
			out.write(PROTO_VERS)
			out.write(2)
			out.write(NO_AUTH)
			out.write(USER_PASSW)
			out.flush()
			Dim data As SByte() = New SByte(1){}
			Dim i As Integer = readSocksReply([in], data, deadlineMillis)
			If i <> 2 OrElse (CInt(data(0))) <> PROTO_VERS Then
				' Maybe it's not a V5 sever after all
				' Let's try V4 before we give up
				' SOCKS Protocol version 4 doesn't know how to deal with
				' DOMAIN type of addresses (unresolved addresses here)
				If epoint.unresolved Then Throw New UnknownHostException(epoint.ToString())
				connectV4([in], out, epoint, deadlineMillis)
				Return
			End If
			If (CInt(data(1))) = NO_METHODS Then Throw New SocketException("SOCKS : No acceptable methods")
			If Not authenticate(data(1), [in], out, deadlineMillis) Then Throw New SocketException("SOCKS : authentication failed")
			out.write(PROTO_VERS)
			out.write(CONNECT)
			out.write(0)
			' Test for IPV4/IPV6/Unresolved 
			If epoint.unresolved Then
				out.write(DOMAIN_NAME)
				out.write(epoint.hostName.length())
				Try
					out.write(epoint.hostName.getBytes("ISO-8859-1"))
				Catch uee As java.io.UnsupportedEncodingException
					Debug.Assert(False)
				End Try
				out.write((epoint.port >> 8) And &Hff)
				out.write((epoint.port >> 0) And &Hff)
			ElseIf TypeOf epoint.address Is Inet6Address Then
				out.write(IPV6)
				out.write(epoint.address.address)
				out.write((epoint.port >> 8) And &Hff)
				out.write((epoint.port >> 0) And &Hff)
			Else
				out.write(IPV4)
				out.write(epoint.address.address)
				out.write((epoint.port >> 8) And &Hff)
				out.write((epoint.port >> 0) And &Hff)
			End If
			out.flush()
			data = New SByte(3){}
			i = readSocksReply([in], data, deadlineMillis)
			If i <> 4 Then Throw New SocketException("Reply from SOCKS server has bad length")
			Dim ex As SocketException = Nothing
			Dim len As Integer
			Dim addr As SByte()
			Select Case data(1)
			Case REQUEST_OK
				' success!
				Select Case data(3)
				Case IPV4
					addr = New SByte(3){}
					i = readSocksReply([in], addr, deadlineMillis)
					If i <> 4 Then Throw New SocketException("Reply from SOCKS server badly formatted")
					data = New SByte(1){}
					i = readSocksReply([in], data, deadlineMillis)
					If i <> 2 Then Throw New SocketException("Reply from SOCKS server badly formatted")
				Case DOMAIN_NAME
					len = data(1)
					Dim host As SByte() = New SByte(len - 1){}
					i = readSocksReply([in], host, deadlineMillis)
					If i <> len Then Throw New SocketException("Reply from SOCKS server badly formatted")
					data = New SByte(1){}
					i = readSocksReply([in], data, deadlineMillis)
					If i <> 2 Then Throw New SocketException("Reply from SOCKS server badly formatted")
				Case IPV6
					len = data(1)
					addr = New SByte(len - 1){}
					i = readSocksReply([in], addr, deadlineMillis)
					If i <> len Then Throw New SocketException("Reply from SOCKS server badly formatted")
					data = New SByte(1){}
					i = readSocksReply([in], data, deadlineMillis)
					If i <> 2 Then Throw New SocketException("Reply from SOCKS server badly formatted")
				Case Else
					ex = New SocketException("Reply from SOCKS server contains wrong code")
				End Select
			Case GENERAL_FAILURE
				ex = New SocketException("SOCKS server general failure")
			Case NOT_ALLOWED
				ex = New SocketException("SOCKS: Connection not allowed by ruleset")
			Case NET_UNREACHABLE
				ex = New SocketException("SOCKS: Network unreachable")
			Case HOST_UNREACHABLE
				ex = New SocketException("SOCKS: Host unreachable")
			Case CONN_REFUSED
				ex = New SocketException("SOCKS: Connection refused")
			Case TTL_EXPIRED
				ex = New SocketException("SOCKS: TTL expired")
			Case CMD_NOT_SUPPORTED
				ex = New SocketException("SOCKS: Command not supported")
			Case ADDR_TYPE_NOT_SUP
				ex = New SocketException("SOCKS: address type not supported")
			End Select
			If ex IsNot Nothing Then
				[in].close()
				out.close()
				Throw ex
			End If
			external_address = epoint
		End Sub

		Private Class PrivilegedActionAnonymousInnerClassHelper2(Of T)
			Implements java.security.PrivilegedAction(Of T)

			Public Overridable Function run() As ProxySelector
					Return ProxySelector.default
			End Function
		End Class

		Private Sub bindV4(ByVal [in] As java.io.InputStream, ByVal out As java.io.OutputStream, ByVal baddr As InetAddress, ByVal lport As Integer)
			If Not(TypeOf baddr Is Inet4Address) Then Throw New SocketException("SOCKS V4 requires IPv4 only addresses")
			MyBase.bind(baddr, lport)
			Dim addr1 As SByte() = baddr.address
			' Test for AnyLocal 
			Dim naddr As InetAddress = baddr
			If naddr.anyLocalAddress Then
				naddr = java.security.AccessController.doPrivileged(New PrivilegedActionAnonymousInnerClassHelper3(Of T)
				addr1 = naddr.address
			End If
			out.write(PROTO_VERS4)
			out.write(BIND)
			out.write((MyBase.localPort >> 8) And &Hff)
			out.write((MyBase.localPort >> 0) And &Hff)
			out.write(addr1)
			Dim userName_Renamed As String = userName
			Try
				out.write(userName_Renamed.getBytes("ISO-8859-1"))
			Catch uee As java.io.UnsupportedEncodingException
				Debug.Assert(False)
			End Try
			out.write(0)
			out.flush()
			Dim data As SByte() = New SByte(7){}
			Dim n As Integer = readSocksReply([in], data)
			If n <> 8 Then Throw New SocketException("Reply from SOCKS server has bad length: " & n)
			If data(0) <> 0 AndAlso data(0) <> 4 Then Throw New SocketException("Reply from SOCKS server has bad version")
			Dim ex As SocketException = Nothing
			Select Case data(1)
			Case 90
				' Success!
				external_address = New InetSocketAddress(baddr, lport)
			Case 91
				ex = New SocketException("SOCKS request rejected")
			Case 92
				ex = New SocketException("SOCKS server couldn't reach destination")
			Case 93
				ex = New SocketException("SOCKS authentication failed")
			Case Else
				ex = New SocketException("Reply from SOCKS server contains bad status")
			End Select
			If ex IsNot Nothing Then
				[in].close()
				out.close()
				Throw ex
			End If

		End Sub

		Private Class PrivilegedActionAnonymousInnerClassHelper3(Of T)
			Implements java.security.PrivilegedAction(Of T)

			Public Overridable Function run() As InetAddress
				Return outerInstance.cmdsock.localAddress

			End Function
		End Class

		''' <summary>
		''' Sends the Bind request to the SOCKS proxy. In the SOCKS protocol, bind
		''' means "accept incoming connection from", so the SocketAddress is the
		''' the one of the host we do accept connection from.
		''' </summary>
		''' <param name="saddr">   the Socket address of the remote host. </param>
		''' <exception cref="IOException">  if an I/O error occurs when binding this socket. </exception>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Protected Friend Overridable Sub socksBind(ByVal saddr As InetSocketAddress)
			If socket_Renamed IsNot Nothing Then Return

			' Connects to the SOCKS server

			If server Is Nothing Then
				' This is the general case
				' server is not null only when the socket was created with a
				' specified proxy in which case it does bypass the ProxySelector
				Dim sel As ProxySelector = java.security.AccessController.doPrivileged(New PrivilegedActionAnonymousInnerClassHelper4(Of T)
				If sel Is Nothing Then Return
				Dim uri As URI
				' Use getHostString() to avoid reverse lookups
				Dim host As String = saddr.hostString
				' IPv6 litteral?
				If TypeOf saddr.address Is Inet6Address AndAlso ((Not host.StartsWith("["))) AndAlso (host.IndexOf(":") >= 0) Then host = "[" & host & "]"
				Try
					uri = New URI("serversocket://" & sun.net.www.ParseUtil.encodePath(host) & ":" & saddr.port)
				Catch e As URISyntaxException
					' This shouldn't happen
					Debug.Assert(False, e)
					uri = Nothing
				End Try
				Dim p As Proxy = Nothing
				Dim savedExc As Exception = Nothing
				Dim iProxy As IEnumerator(Of Proxy) = Nothing
				iProxy = sel.select(uri).GetEnumerator()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				If iProxy Is Nothing OrElse Not(iProxy.hasNext()) Then Return
				Do While iProxy.MoveNext()
					p = iProxy.Current
					If p Is Nothing OrElse p.type() <> Proxy.Type.SOCKS Then Return

					If Not(TypeOf p.address() Is InetSocketAddress) Then Throw New SocketException("Unknown address type for proxy: " & p)
					' Use getHostString() to avoid reverse lookups
					server = CType(p.address(), InetSocketAddress).hostString
					serverPort = CType(p.address(), InetSocketAddress).port
					If TypeOf p Is sun.net.SocksProxy Then
						If CType(p, sun.net.SocksProxy).protocolVersion() = 4 Then useV4 = True
					End If

					' Connects to the SOCKS server
					Try
						java.security.AccessController.doPrivileged(New PrivilegedExceptionActionAnonymousInnerClassHelper2(Of T)
					Catch e As Exception
						' Ooops, let's notify the ProxySelector
						sel.connectFailed(uri,p.address(),New SocketException(e.message))
						server = Nothing
						serverPort = -1
						cmdsock = Nothing
						savedExc = e
						' Will continue the while loop and try the next proxy
					End Try
				Loop

	'            
	'             * If server is still null at this point, none of the proxy
	'             * worked
	'             
				If server Is Nothing OrElse cmdsock Is Nothing Then Throw New SocketException("Can't connect to SOCKS proxy:" & savedExc.message)
			Else
				Try
					java.security.AccessController.doPrivileged(New PrivilegedExceptionActionAnonymousInnerClassHelper3(Of T)
				Catch e As Exception
					Throw New SocketException(e.message)
				End Try
			End If
			Dim out As New java.io.BufferedOutputStream(cmdOut, 512)
			Dim [in] As java.io.InputStream = cmdIn
			If useV4 Then
				bindV4([in], out, saddr.address, saddr.port)
				Return
			End If
			out.write(PROTO_VERS)
			out.write(2)
			out.write(NO_AUTH)
			out.write(USER_PASSW)
			out.flush()
			Dim data As SByte() = New SByte(1){}
			Dim i As Integer = readSocksReply([in], data)
			If i <> 2 OrElse (CInt(data(0))) <> PROTO_VERS Then
				' Maybe it's not a V5 sever after all
				' Let's try V4 before we give up
				bindV4([in], out, saddr.address, saddr.port)
				Return
			End If
			If (CInt(data(1))) = NO_METHODS Then Throw New SocketException("SOCKS : No acceptable methods")
			If Not authenticate(data(1), [in], out) Then Throw New SocketException("SOCKS : authentication failed")
			' We're OK. Let's issue the BIND command.
			out.write(PROTO_VERS)
			out.write(BIND)
			out.write(0)
			Dim lport As Integer = saddr.port
			If saddr.unresolved Then
				out.write(DOMAIN_NAME)
				out.write(saddr.hostName.length())
				Try
					out.write(saddr.hostName.getBytes("ISO-8859-1"))
				Catch uee As java.io.UnsupportedEncodingException
					Debug.Assert(False)
				End Try
				out.write((lport >> 8) And &Hff)
				out.write((lport >> 0) And &Hff)
			ElseIf TypeOf saddr.address Is Inet4Address Then
				Dim addr1 As SByte() = saddr.address.address
				out.write(IPV4)
				out.write(addr1)
				out.write((lport >> 8) And &Hff)
				out.write((lport >> 0) And &Hff)
				out.flush()
			ElseIf TypeOf saddr.address Is Inet6Address Then
				Dim addr1 As SByte() = saddr.address.address
				out.write(IPV6)
				out.write(addr1)
				out.write((lport >> 8) And &Hff)
				out.write((lport >> 0) And &Hff)
				out.flush()
			Else
				cmdsock.close()
				Throw New SocketException("unsupported address type : " & saddr)
			End If
			data = New SByte(3){}
			i = readSocksReply([in], data)
			Dim ex As SocketException = Nothing
			Dim len, nport As Integer
			Dim addr As SByte()
			Select Case data(1)
			Case REQUEST_OK
				' success!
				Select Case data(3)
				Case IPV4
					addr = New SByte(3){}
					i = readSocksReply([in], addr)
					If i <> 4 Then Throw New SocketException("Reply from SOCKS server badly formatted")
					data = New SByte(1){}
					i = readSocksReply([in], data)
					If i <> 2 Then Throw New SocketException("Reply from SOCKS server badly formatted")
					nport = (CInt(data(0)) And &Hff) << 8
					nport += (CInt(data(1)) And &Hff)
					external_address = New InetSocketAddress(New Inet4Address("", addr), nport)
				Case DOMAIN_NAME
					len = data(1)
					Dim host As SByte() = New SByte(len - 1){}
					i = readSocksReply([in], host)
					If i <> len Then Throw New SocketException("Reply from SOCKS server badly formatted")
					data = New SByte(1){}
					i = readSocksReply([in], data)
					If i <> 2 Then Throw New SocketException("Reply from SOCKS server badly formatted")
					nport = (CInt(data(0)) And &Hff) << 8
					nport += (CInt(data(1)) And &Hff)
					external_address = New InetSocketAddress(New String(host), nport)
				Case IPV6
					len = data(1)
					addr = New SByte(len - 1){}
					i = readSocksReply([in], addr)
					If i <> len Then Throw New SocketException("Reply from SOCKS server badly formatted")
					data = New SByte(1){}
					i = readSocksReply([in], data)
					If i <> 2 Then Throw New SocketException("Reply from SOCKS server badly formatted")
					nport = (CInt(data(0)) And &Hff) << 8
					nport += (CInt(data(1)) And &Hff)
					external_address = New InetSocketAddress(New Inet6Address("", addr), nport)
				End Select
			Case GENERAL_FAILURE
				ex = New SocketException("SOCKS server general failure")
			Case NOT_ALLOWED
				ex = New SocketException("SOCKS: Bind not allowed by ruleset")
			Case NET_UNREACHABLE
				ex = New SocketException("SOCKS: Network unreachable")
			Case HOST_UNREACHABLE
				ex = New SocketException("SOCKS: Host unreachable")
			Case CONN_REFUSED
				ex = New SocketException("SOCKS: Connection refused")
			Case TTL_EXPIRED
				ex = New SocketException("SOCKS: TTL expired")
			Case CMD_NOT_SUPPORTED
				ex = New SocketException("SOCKS: Command not supported")
			Case ADDR_TYPE_NOT_SUP
				ex = New SocketException("SOCKS: address type not supported")
			End Select
			If ex IsNot Nothing Then
				[in].close()
				out.close()
				cmdsock.close()
				cmdsock = Nothing
				Throw ex
			End If
			cmdIn = [in]
			cmdOut = out
		End Sub

		Private Class PrivilegedActionAnonymousInnerClassHelper4(Of T)
			Implements java.security.PrivilegedAction(Of T)

			Public Overridable Function run() As ProxySelector
					Return ProxySelector.default
			End Function
		End Class

		Private Class PrivilegedExceptionActionAnonymousInnerClassHelper2(Of T)
			Implements java.security.PrivilegedExceptionAction(Of T)

			Public Overridable Function run() As Void
				outerInstance.cmdsock = New Socket(New PlainSocketImpl)
				outerInstance.cmdsock.connect(New InetSocketAddress(outerInstance.server, outerInstance.serverPort))
				outerInstance.cmdIn = outerInstance.cmdsock.inputStream
				outerInstance.cmdOut = outerInstance.cmdsock.outputStream
				Return Nothing
			End Function
		End Class

		Private Class PrivilegedExceptionActionAnonymousInnerClassHelper3(Of T)
			Implements java.security.PrivilegedExceptionAction(Of T)

			Public Overridable Function run() As Void
				outerInstance.cmdsock = New Socket(New PlainSocketImpl)
				outerInstance.cmdsock.connect(New InetSocketAddress(outerInstance.server, outerInstance.serverPort))
				outerInstance.cmdIn = outerInstance.cmdsock.inputStream
				outerInstance.cmdOut = outerInstance.cmdsock.outputStream
				Return Nothing
			End Function
		End Class

		''' <summary>
		''' Accepts a connection from a specific host.
		''' </summary>
		''' <param name="s">   the accepted connection. </param>
		''' <param name="saddr"> the socket address of the host we do accept
		'''               connection from </param>
		''' <exception cref="IOException">  if an I/O error occurs when accepting the
		'''               connection. </exception>
		Protected Friend Overridable Sub acceptFrom(ByVal s As SocketImpl, ByVal saddr As InetSocketAddress)
			If cmdsock Is Nothing Then Return
			Dim [in] As java.io.InputStream = cmdIn
			' Sends the "SOCKS BIND" request.
			socksBind(saddr)
			[in].read()
			Dim i As Integer = [in].read()
			[in].read()
			Dim ex As SocketException = Nothing
			Dim nport As Integer
			Dim addr As SByte()
			Dim real_end As InetSocketAddress = Nothing
			Select Case i
			Case REQUEST_OK
				' success!
				i = [in].read()
				Select Case i
				Case IPV4
					addr = New SByte(3){}
					readSocksReply([in], addr)
					nport = [in].read() << 8
					nport += [in].read()
					real_end = New InetSocketAddress(New Inet4Address("", addr), nport)
				Case DOMAIN_NAME
					Dim len As Integer = [in].read()
					addr = New SByte(len - 1){}
					readSocksReply([in], addr)
					nport = [in].read() << 8
					nport += [in].read()
					real_end = New InetSocketAddress(New String(addr), nport)
				Case IPV6
					addr = New SByte(15){}
					readSocksReply([in], addr)
					nport = [in].read() << 8
					nport += [in].read()
					real_end = New InetSocketAddress(New Inet6Address("", addr), nport)
				End Select
			Case GENERAL_FAILURE
				ex = New SocketException("SOCKS server general failure")
			Case NOT_ALLOWED
				ex = New SocketException("SOCKS: Accept not allowed by ruleset")
			Case NET_UNREACHABLE
				ex = New SocketException("SOCKS: Network unreachable")
			Case HOST_UNREACHABLE
				ex = New SocketException("SOCKS: Host unreachable")
			Case CONN_REFUSED
				ex = New SocketException("SOCKS: Connection refused")
			Case TTL_EXPIRED
				ex = New SocketException("SOCKS: TTL expired")
			Case CMD_NOT_SUPPORTED
				ex = New SocketException("SOCKS: Command not supported")
			Case ADDR_TYPE_NOT_SUP
				ex = New SocketException("SOCKS: address type not supported")
			End Select
			If ex IsNot Nothing Then
				cmdIn.close()
				cmdOut.close()
				cmdsock.close()
				cmdsock = Nothing
				Throw ex
			End If

			''' <summary>
			''' This is where we have to do some fancy stuff.
			''' The datastream from the socket "accepted" by the proxy will
			''' come through the cmdSocket. So we have to swap the socketImpls
			''' </summary>
			If TypeOf s Is SocksSocketImpl Then CType(s, SocksSocketImpl).external_address = real_end
			If TypeOf s Is PlainSocketImpl Then
				Dim psi As PlainSocketImpl = CType(s, PlainSocketImpl)
				psi.inputStream = CType([in], SocketInputStream)
				psi.fileDescriptor = cmdsock.impl.fileDescriptor
				psi.address = cmdsock.impl.inetAddress
				psi.port = cmdsock.impl.port
				psi.localPort = cmdsock.impl.localPort
			Else
				s.fd = cmdsock.impl.fd
				s.address = cmdsock.impl.address
				s.port = cmdsock.impl.port
				s.localport = cmdsock.impl.localport
			End If

			' Need to do that so that the socket won't be closed
			' when the ServerSocket is closed by the user.
			' It kinds of detaches the Socket because it is now
			' used elsewhere.
			cmdsock = Nothing
		End Sub


		''' <summary>
		''' Returns the value of this socket's {@code address} field.
		''' </summary>
		''' <returns>  the value of this socket's {@code address} field. </returns>
		''' <seealso cref=     java.net.SocketImpl#address </seealso>
		Protected Friend Property Overrides inetAddress As InetAddress
			Get
				If external_address IsNot Nothing Then
					Return external_address.address
				Else
					Return MyBase.inetAddress
				End If
			End Get
		End Property

		''' <summary>
		''' Returns the value of this socket's {@code port} field.
		''' </summary>
		''' <returns>  the value of this socket's {@code port} field. </returns>
		''' <seealso cref=     java.net.SocketImpl#port </seealso>
		Protected Friend Property Overrides port As Integer
			Get
				If external_address IsNot Nothing Then
					Return external_address.port
				Else
					Return MyBase.port
				End If
			End Get
		End Property

		Protected Friend Property Overrides localPort As Integer
			Get
				If socket_Renamed IsNot Nothing Then Return MyBase.localPort
				If external_address IsNot Nothing Then
					Return external_address.port
				Else
					Return MyBase.localPort
				End If
			End Get
		End Property

		Protected Friend Overrides Sub close()
			If cmdsock IsNot Nothing Then cmdsock.close()
			cmdsock = Nothing
			MyBase.close()
		End Sub

		Private Property userName As String
			Get
				Dim userName_Renamed As String = ""
				If applicationSetProxy Then
					Try
						userName_Renamed = System.getProperty("user.name") ' swallow Exception
					Catch se As SecurityException
					End Try
				Else
					userName_Renamed = java.security.AccessController.doPrivileged(New sun.security.action.GetPropertyAction("user.name"))
				End If
				Return userName_Renamed
			End Get
		End Property
	End Class

End Namespace