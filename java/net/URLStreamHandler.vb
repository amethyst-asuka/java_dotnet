Imports System
Imports System.Runtime.CompilerServices

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
	''' The abstract class {@code URLStreamHandler} is the common
	''' superclass for all stream protocol handlers. A stream protocol
	''' handler knows how to make a connection for a particular protocol
	''' type, such as {@code http} or {@code https}.
	''' <p>
	''' In most cases, an instance of a {@code URLStreamHandler}
	''' subclass is not created directly by an application. Rather, the
	''' first time a protocol name is encountered when constructing a
	''' {@code URL}, the appropriate stream protocol handler is
	''' automatically loaded.
	''' 
	''' @author  James Gosling </summary>
	''' <seealso cref=     java.net.URL#URL(java.lang.String, java.lang.String, int, java.lang.String)
	''' @since   JDK1.0 </seealso>
	Public MustInherit Class URLStreamHandler
		''' <summary>
		''' Opens a connection to the object referenced by the
		''' {@code URL} argument.
		''' This method should be overridden by a subclass.
		''' 
		''' <p>If for the handler's protocol (such as HTTP or JAR), there
		''' exists a public, specialized URLConnection subclass belonging
		''' to one of the following packages or one of their subpackages:
		''' java.lang, java.io, java.util, java.net, the connection
		''' returned will be of that subclass. For example, for HTTP an
		''' HttpURLConnection will be returned, and for JAR a
		''' JarURLConnection will be returned.
		''' </summary>
		''' <param name="u">   the URL that this connects to. </param>
		''' <returns>     a {@code URLConnection} object for the {@code URL}. </returns>
		''' <exception cref="IOException">  if an I/O error occurs while opening the
		'''               connection. </exception>
		Protected Friend MustOverride Function openConnection(  u As URL) As URLConnection

		''' <summary>
		''' Same as openConnection(URL), except that the connection will be
		''' made through the specified proxy; Protocol handlers that do not
		''' support proxying will ignore the proxy parameter and make a
		''' normal connection.
		''' 
		''' Calling this method preempts the system's default ProxySelector
		''' settings.
		''' </summary>
		''' <param name="u">   the URL that this connects to. </param>
		''' <param name="p">   the proxy through which the connection will be made.
		'''                 If direct connection is desired, Proxy.NO_PROXY
		'''                 should be specified. </param>
		''' <returns>     a {@code URLConnection} object for the {@code URL}. </returns>
		''' <exception cref="IOException">  if an I/O error occurs while opening the
		'''               connection. </exception>
		''' <exception cref="IllegalArgumentException"> if either u or p is null,
		'''               or p has the wrong type. </exception>
		''' <exception cref="UnsupportedOperationException"> if the subclass that
		'''               implements the protocol doesn't support this method.
		''' @since      1.5 </exception>
		Protected Friend Overridable Function openConnection(  u As URL,   p As Proxy) As URLConnection
			Throw New UnsupportedOperationException("Method not implemented.")
		End Function

		''' <summary>
		''' Parses the string representation of a {@code URL} into a
		''' {@code URL} object.
		''' <p>
		''' If there is any inherited context, then it has already been
		''' copied into the {@code URL} argument.
		''' <p>
		''' The {@code parseURL} method of {@code URLStreamHandler}
		''' parses the string representation as if it were an
		''' {@code http} specification. Most URL protocol families have a
		''' similar parsing. A stream protocol handler for a protocol that has
		''' a different syntax must override this routine.
		''' </summary>
		''' <param name="u">       the {@code URL} to receive the result of parsing
		'''                  the spec. </param>
		''' <param name="spec">    the {@code String} representing the URL that
		'''                  must be parsed. </param>
		''' <param name="start">   the character index at which to begin parsing. This is
		'''                  just past the '{@code :}' (if there is one) that
		'''                  specifies the determination of the protocol name. </param>
		''' <param name="limit">   the character position to stop parsing at. This is the
		'''                  end of the string or the position of the
		'''                  "{@code #}" character, if present. All information
		'''                  after the sharp sign indicates an anchor. </param>
		Protected Friend Overridable Sub parseURL(  u As URL,   spec As String,   start As Integer,   limit As Integer)
			' These fields may receive context content if this was relative URL
			Dim protocol As String = u.protocol
			Dim authority As String = u.authority
			Dim userInfo As String = u.userInfo
			Dim host As String = u.host
			Dim port As Integer = u.port
			Dim path As String = u.path
			Dim query As String = u.query

			' This field has already been parsed
			Dim ref As String = u.ref

			Dim isRelPath As Boolean = False
			Dim queryOnly As Boolean = False

	' FIX: should not assume query if opaque
			' Strip off the query part
			If start < limit Then
				Dim queryStart As Integer = spec.IndexOf("?"c)
				queryOnly = queryStart = start
				If (queryStart <> -1) AndAlso (queryStart < limit) Then
					query = spec.Substring(queryStart+1, limit - (queryStart+1))
					If limit > queryStart Then limit = queryStart
					spec = spec.Substring(0, queryStart)
				End If
			End If

			Dim i As Integer = 0
			' Parse the authority part if any
			Dim isUNCName As Boolean = (start <= limit - 4) AndAlso (spec.Chars(start) = "/"c) AndAlso (spec.Chars(start + 1) = "/"c) AndAlso (spec.Chars(start + 2) = "/"c) AndAlso (spec.Chars(start + 3) = "/"c)
			If (Not isUNCName) AndAlso (start <= limit - 2) AndAlso (spec.Chars(start) = "/"c) AndAlso (spec.Chars(start + 1) = "/"c) Then
				start += 2
				i = spec.IndexOf("/"c, start)
				If i < 0 Then
					i = spec.IndexOf("?"c, start)
					If i < 0 Then i = limit
				End If

					authority = spec.Substring(start, i - start)
					host = authority

				Dim ind As Integer = authority.IndexOf("@"c)
				If ind <> -1 Then
					userInfo = authority.Substring(0, ind)
					host = authority.Substring(ind+1)
				Else
					userInfo = Nothing
				End If
				If host IsNot Nothing Then
					' If the host is surrounded by [ and ] then its an IPv6
					' literal address as specified in RFC2732
					If host.length()>0 AndAlso (host.Chars(0) = "["c) Then
						ind = host.IndexOf("]"c)
						If ind > 2 Then

							Dim nhost As String = host
							host = nhost.Substring(0,ind+1)
							If Not sun.net.util.IPAddressUtil.isIPv6LiteralAddress(host.Substring(1, ind - 1)) Then Throw New IllegalArgumentException("Invalid host: " & host)

							port = -1
							If nhost.length() > ind+1 Then
								If nhost.Chars(ind+1) = ":"c Then
									ind += 1
									' port can be null according to RFC2396
									If nhost.length() > (ind + 1) Then port = Convert.ToInt32(nhost.Substring(ind+1))
								Else
									Throw New IllegalArgumentException("Invalid authority field: " & authority)
								End If
							End If
						Else
							Throw New IllegalArgumentException("Invalid authority field: " & authority)
						End If
					Else
						ind = host.IndexOf(":"c)
						port = -1
						If ind >= 0 Then
							' port can be null according to RFC2396
							If host.length() > (ind + 1) Then port = Convert.ToInt32(host.Substring(ind + 1))
							host = host.Substring(0, ind)
						End If
					End If
				Else
					host = ""
				End If
				If port < -1 Then Throw New IllegalArgumentException("Invalid port number :" & port)
				start = i
				' If the authority is defined then the path is defined by the
				' spec only; See RFC 2396 Section 5.2.4.
				If authority IsNot Nothing AndAlso authority.length() > 0 Then path = ""
			End If

			If host Is Nothing Then host = ""

			' Parse the file path if any
			If start < limit Then
				If spec.Chars(start) = "/"c Then
					path = spec.Substring(start, limit - start)
				ElseIf path IsNot Nothing AndAlso path.length() > 0 Then
					isRelPath = True
					Dim ind As Integer = path.LastIndexOf("/"c)
					Dim seperator As String = ""
					If ind = -1 AndAlso authority IsNot Nothing Then seperator = "/"
					path = path.Substring(0, ind + 1) + seperator + spec.Substring(start, limit - start)

				Else
					Dim seperator As String = If(authority IsNot Nothing, "/", "")
					path = seperator + spec.Substring(start, limit - start)
				End If
			ElseIf queryOnly AndAlso path IsNot Nothing Then
				Dim ind As Integer = path.LastIndexOf("/"c)
				If ind < 0 Then ind = 0
				path = path.Substring(0, ind) & "/"
			End If
			If path Is Nothing Then path = ""

			If isRelPath Then
				' Remove embedded /./
				i = path.IndexOf("/./")
				Do While i >= 0
					path = path.Substring(0, i) + path.Substring(i + 2)
					i = path.IndexOf("/./")
				Loop
				' Remove embedded /../ if possible
				i = 0
				i = path.IndexOf("/../", i)
				Do While i >= 0
	'                
	'                 * A "/../" will cancel the previous segment and itself,
	'                 * unless that segment is a "/../" itself
	'                 * i.e. "/a/b/../c" becomes "/a/c"
	'                 * but "/../../a" should stay unchanged
	'                 
					limit = path.LastIndexOf("/"c, i - 1)
					If i > 0 AndAlso limit >= 0 AndAlso (path.IndexOf("/../", limit) <> 0) Then
						path = path.Substring(0, limit) + path.Substring(i + 3)
						i = 0
					Else
						i = i + 3
					End If
					i = path.IndexOf("/../", i)
				Loop
				' Remove trailing .. if possible
				Do While path.EndsWith("/..")
					i = path.IndexOf("/..")
					limit = path.LastIndexOf("/"c, i - 1)
					If limit >= 0 Then
						path = path.Substring(0, limit+1)
					Else
						Exit Do
					End If
				Loop
				' Remove starting .
				If path.StartsWith("./") AndAlso path.length() > 2 Then path = path.Substring(2)

				' Remove trailing .
				If path.EndsWith("/.") Then path = path.Substring(0, path.length() -1)
			End If

			uRLURL(u, protocol, host, port, authority, userInfo, path, query, ref)
		End Sub

		''' <summary>
		''' Returns the default port for a URL parsed by this handler. This method
		''' is meant to be overidden by handlers with default port numbers. </summary>
		''' <returns> the default port for a {@code URL} parsed by this handler.
		''' @since 1.3 </returns>
		Protected Friend Overridable Property defaultPort As Integer
			Get
				Return -1
			End Get
		End Property

		''' <summary>
		''' Provides the default equals calculation. May be overidden by handlers
		''' for other protocols that have different requirements for equals().
		''' This method requires that none of its arguments is null. This is
		''' guaranteed by the fact that it is only called by java.net.URL class. </summary>
		''' <param name="u1"> a URL object </param>
		''' <param name="u2"> a URL object </param>
		''' <returns> {@code true} if the two urls are
		''' considered equal, ie. they refer to the same
		''' fragment in the same file.
		''' @since 1.3 </returns>
		Protected Friend Overrides Function Equals(  u1 As URL,   u2 As URL) As Boolean
			Dim ref1 As String = u1.ref
			Dim ref2 As String = u2.ref
			Return (ref1 = ref2 OrElse (ref1 IsNot Nothing AndAlso ref1.Equals(ref2))) AndAlso sameFile(u1, u2)
		End Function

		''' <summary>
		''' Provides the default hash calculation. May be overidden by handlers for
		''' other protocols that have different requirements for hashCode
		''' calculation. </summary>
		''' <param name="u"> a URL object </param>
		''' <returns> an {@code int} suitable for hash table indexing
		''' @since 1.3 </returns>
		Protected Friend Overrides Function GetHashCode(  u As URL) As Integer
			Dim h As Integer = 0

			' Generate the protocol part.
			Dim protocol As String = u.protocol
			If protocol IsNot Nothing Then h += protocol.GetHashCode()

			' Generate the host part.
			Dim addr As InetAddress = getHostAddress(u)
			If addr IsNot Nothing Then
				h += addr.GetHashCode()
			Else
				Dim host As String = u.host
				If host IsNot Nothing Then h += host.ToLower().GetHashCode()
			End If

			' Generate the file part.
			Dim file As String = u.file
			If file IsNot Nothing Then h += file.GetHashCode()

			' Generate the port part.
			If u.port = -1 Then
				h += defaultPort
			Else
				h += u.port
			End If

			' Generate the ref part.
			Dim ref As String = u.ref
			If ref IsNot Nothing Then h += ref.GetHashCode()

			Return h
		End Function

		''' <summary>
		''' Compare two urls to see whether they refer to the same file,
		''' i.e., having the same protocol, host, port, and path.
		''' This method requires that none of its arguments is null. This is
		''' guaranteed by the fact that it is only called indirectly
		''' by java.net.URL class. </summary>
		''' <param name="u1"> a URL object </param>
		''' <param name="u2"> a URL object </param>
		''' <returns> true if u1 and u2 refer to the same file
		''' @since 1.3 </returns>
		Protected Friend Overridable Function sameFile(  u1 As URL,   u2 As URL) As Boolean
			' Compare the protocols.
			If Not((u1.protocol = u2.protocol) OrElse (u1.protocol IsNot Nothing AndAlso u1.protocol.equalsIgnoreCase(u2.protocol))) Then Return False

			' Compare the files.
			If Not(u1.file = u2.file OrElse (u1.file IsNot Nothing AndAlso u1.file.Equals(u2.file))) Then Return False

			' Compare the ports.
			Dim port1, port2 As Integer
			port1 = If(u1.port <> -1, u1.port, u1.handler.defaultPort)
			port2 = If(u2.port <> -1, u2.port, u2.handler.defaultPort)
			If port1 <> port2 Then Return False

			' Compare the hosts.
			If Not hostsEqual(u1, u2) Then Return False

			Return True
		End Function

		''' <summary>
		''' Get the IP address of our host. An empty host field or a DNS failure
		''' will result in a null return.
		''' </summary>
		''' <param name="u"> a URL object </param>
		''' <returns> an {@code InetAddress} representing the host
		''' IP address.
		''' @since 1.3 </returns>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Protected Friend Overridable Function getHostAddress(  u As URL) As InetAddress
			If u.hostAddress IsNot Nothing Then Return u.hostAddress

			Dim host As String = u.host
			If host Is Nothing OrElse host.Equals("") Then
				Return Nothing
			Else
				Try
					u.hostAddress = InetAddress.getByName(host)
				Catch ex As UnknownHostException
					Return Nothing
				Catch se As SecurityException
					Return Nothing
				End Try
			End If
			Return u.hostAddress
		End Function

		''' <summary>
		''' Compares the host components of two URLs. </summary>
		''' <param name="u1"> the URL of the first host to compare </param>
		''' <param name="u2"> the URL of the second host to compare </param>
		''' <returns>  {@code true} if and only if they
		''' are equal, {@code false} otherwise.
		''' @since 1.3 </returns>
		Protected Friend Overridable Function hostsEqual(  u1 As URL,   u2 As URL) As Boolean
			Dim a1 As InetAddress = getHostAddress(u1)
			Dim a2 As InetAddress = getHostAddress(u2)
			' if we have internet address for both, compare them
			If a1 IsNot Nothing AndAlso a2 IsNot Nothing Then
				Return a1.Equals(a2)
			' else, if both have host names, compare them
			ElseIf u1.host IsNot Nothing AndAlso u2.host IsNot Nothing Then
				Return u1.host.equalsIgnoreCase(u2.host)
			 Else
				Return u1.host Is Nothing AndAlso u2.host Is Nothing
			 End If
		End Function

		''' <summary>
		''' Converts a {@code URL} of a specific protocol to a
		''' {@code String}.
		''' </summary>
		''' <param name="u">   the URL. </param>
		''' <returns>  a string representation of the {@code URL} argument. </returns>
		Protected Friend Overridable Function toExternalForm(  u As URL) As String

			' pre-compute length of StringBuffer
			Dim len As Integer = u.protocol.length() + 1
			If u.authority IsNot Nothing AndAlso u.authority.length() > 0 Then len += 2 + u.authority.length()
			If u.path IsNot Nothing Then len += u.path.length()
			If u.query IsNot Nothing Then len += 1 + u.query.length()
			If u.ref IsNot Nothing Then len += 1 + u.ref.length()

			Dim result As New StringBuffer(len)
			result.append(u.protocol)
			result.append(":")
			If u.authority IsNot Nothing AndAlso u.authority.length() > 0 Then
				result.append("//")
				result.append(u.authority)
			End If
			If u.path IsNot Nothing Then result.append(u.path)
			If u.query IsNot Nothing Then
				result.append("?"c)
				result.append(u.query)
			End If
			If u.ref IsNot Nothing Then
				result.append("#")
				result.append(u.ref)
			End If
			Return result.ToString()
		End Function

		''' <summary>
		''' Sets the fields of the {@code URL} argument to the indicated values.
		''' Only classes derived from URLStreamHandler are able
		''' to use this method to set the values of the URL fields.
		''' </summary>
		''' <param name="u">         the URL to modify. </param>
		''' <param name="protocol">  the protocol name. </param>
		''' <param name="host">      the remote host value for the URL. </param>
		''' <param name="port">      the port on the remote machine. </param>
		''' <param name="authority"> the authority part for the URL. </param>
		''' <param name="userInfo"> the userInfo part of the URL. </param>
		''' <param name="path">      the path component of the URL. </param>
		''' <param name="query">     the query part for the URL. </param>
		''' <param name="ref">       the reference. </param>
		''' <exception cref="SecurityException">       if the protocol handler of the URL is
		'''                                  different from this one </exception>
		''' <seealso cref=     java.net.URL#set(java.lang.String, java.lang.String, int, java.lang.String, java.lang.String)
		''' @since 1.3 </seealso>
		   Protected Friend Overridable Sub setURL(  u As URL,   protocol As String,   host As String,   port As Integer,   authority As String,   userInfo As String,   path As String,   query As String,   ref As String)
			If Me IsNot u.handler Then Throw New SecurityException("handler for url different from " & "this handler")
			' ensure that no one can reset the protocol on a given URL.
			u.set(u.protocol, host, port, authority, userInfo, path, query, ref)
		   End Sub

		''' <summary>
		''' Sets the fields of the {@code URL} argument to the indicated values.
		''' Only classes derived from URLStreamHandler are able
		''' to use this method to set the values of the URL fields.
		''' </summary>
		''' <param name="u">         the URL to modify. </param>
		''' <param name="protocol">  the protocol name. This value is ignored since 1.2. </param>
		''' <param name="host">      the remote host value for the URL. </param>
		''' <param name="port">      the port on the remote machine. </param>
		''' <param name="file">      the file. </param>
		''' <param name="ref">       the reference. </param>
		''' <exception cref="SecurityException">       if the protocol handler of the URL is
		'''                                  different from this one </exception>
		''' @deprecated Use setURL(URL, String, String, int, String, String, String,
		'''             String); 
		<Obsolete("Use setURL(URL, String, String, int, String, String, String,")> _
		Protected Friend Overridable Sub setURL(  u As URL,   protocol As String,   host As String,   port As Integer,   file As String,   ref As String)
	'        
	'         * Only old URL handlers call this, so assume that the host
	'         * field might contain "user:passwd@host". Fix as necessary.
	'         
			Dim authority As String = Nothing
			Dim userInfo As String = Nothing
			If host IsNot Nothing AndAlso host.length() <> 0 Then
				authority = If(port = -1, host, host & ":" & port)
				Dim at As Integer = host.LastIndexOf("@"c)
				If at <> -1 Then
					userInfo = host.Substring(0, at)
					host = host.Substring(at+1)
				End If
			End If

	'        
	'         * Assume file might contain query part. Fix as necessary.
	'         
			Dim path As String = Nothing
			Dim query As String = Nothing
			If file IsNot Nothing Then
				Dim q As Integer = file.LastIndexOf("?"c)
				If q <> -1 Then
					query = file.Substring(q+1)
					path = file.Substring(0, q)
				Else
					path = file
				End If
			End If
			uRLURL(u, protocol, host, port, authority, userInfo, path, query, ref)
		End Sub
	End Class

End Namespace