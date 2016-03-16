Imports Microsoft.VisualBasic
Imports System
Imports System.Runtime.CompilerServices
Imports System.Collections
Imports System.Collections.Generic

'
' * Copyright (c) 1997, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' This class represents access to a network via sockets.
	''' A SocketPermission consists of a
	''' host specification and a set of "actions" specifying ways to
	''' connect to that host. The host is specified as
	''' <pre>
	'''    host = (hostname | IPv4address | iPv6reference) [:portrange]
	'''    portrange = portnumber | -portnumber | portnumber-[portnumber]
	''' </pre>
	''' The host is expressed as a DNS name, as a numerical IP address,
	''' or as "localhost" (for the local machine).
	''' The wildcard "*" may be included once in a DNS name host
	''' specification. If it is included, it must be in the leftmost
	''' position, as in "*.sun.com".
	''' <p>
	''' The format of the IPv6reference should follow that specified in <a
	''' href="http://www.ietf.org/rfc/rfc2732.txt"><i>RFC&nbsp;2732: Format
	''' for Literal IPv6 Addresses in URLs</i></a>:
	''' <pre>
	'''    ipv6reference = "[" IPv6address "]"
	''' </pre>
	''' For example, you can construct a SocketPermission instance
	''' as the following:
	''' <pre>
	'''    String hostAddress = inetaddress.getHostAddress();
	'''    if (inetaddress instanceof Inet6Address) {
	'''        sp = new SocketPermission("[" + hostAddress + "]:" + port, action);
	'''    } else {
	'''        sp = new SocketPermission(hostAddress + ":" + port, action);
	'''    }
	''' </pre>
	''' or
	''' <pre>
	'''    String host = url.getHost();
	'''    sp = new SocketPermission(host + ":" + port, action);
	''' </pre>
	''' <p>
	''' The <A HREF="Inet6Address.html#lform">full uncompressed form</A> of
	''' an IPv6 literal address is also valid.
	''' <p>
	''' The port or portrange is optional. A port specification of the
	''' form "N-", where <i>N</i> is a port number, signifies all ports
	''' numbered <i>N</i> and above, while a specification of the
	''' form "-N" indicates all ports numbered <i>N</i> and below.
	''' The special port value {@code 0} refers to the entire <i>ephemeral</i>
	''' port range. This is a fixed range of ports a system may use to
	''' allocate dynamic ports from. The actual range may be system dependent.
	''' <p>
	''' The possible ways to connect to the host are
	''' <pre>
	''' accept
	''' connect
	''' listen
	''' resolve
	''' </pre>
	''' The "listen" action is only meaningful when used with "localhost" and
	''' means the ability to bind to a specified port.
	''' The "resolve" action is implied when any of the other actions are present.
	''' The action "resolve" refers to host/ip name service lookups.
	''' <P>
	''' The actions string is converted to lowercase before processing.
	''' <p>As an example of the creation and meaning of SocketPermissions,
	''' note that if the following permission:
	''' 
	''' <pre>
	'''   p1 = new SocketPermission("puffin.eng.sun.com:7777", "connect,accept");
	''' </pre>
	''' 
	''' is granted to some code, it allows that code to connect to port 7777 on
	''' {@code puffin.eng.sun.com}, and to accept connections on that port.
	''' 
	''' <p>Similarly, if the following permission:
	''' 
	''' <pre>
	'''   p2 = new SocketPermission("localhost:1024-", "accept,connect,listen");
	''' </pre>
	''' 
	''' is granted to some code, it allows that code to
	''' accept connections on, connect to, or listen on any port between
	''' 1024 and 65535 on the local host.
	''' 
	''' <p>Note: Granting code permission to accept or make connections to remote
	''' hosts may be dangerous because malevolent code can then more easily
	''' transfer and share confidential data among parties who may not
	''' otherwise have access to the data.
	''' </summary>
	''' <seealso cref= java.security.Permissions </seealso>
	''' <seealso cref= SocketPermission
	''' 
	''' 
	''' @author Marianne Mueller
	''' @author Roland Schemers
	''' 
	''' @serial exclude </seealso>

	<Serializable> _
	Public NotInheritable Class SocketPermission
		Inherits java.security.Permission

		Private Const serialVersionUID As Long = -7204263841984476862L

		''' <summary>
		''' Connect to host:port
		''' </summary>
		Private Const CONNECT As Integer = &H1

		''' <summary>
		''' Listen on host:port
		''' </summary>
		Private Const LISTEN As Integer = &H2

		''' <summary>
		''' Accept a connection from host:port
		''' </summary>
		Private Const ACCEPT As Integer = &H4

		''' <summary>
		''' Resolve DNS queries
		''' </summary>
		Private Const RESOLVE As Integer = &H8

		''' <summary>
		''' No actions
		''' </summary>
		Private Const NONE As Integer = &H0

		''' <summary>
		''' All actions
		''' </summary>
		Private Shared ReadOnly ALL As Integer = CONNECT Or LISTEN Or ACCEPT Or RESOLVE

		' various port constants
		Private Const PORT_MIN As Integer = 0
		Private Const PORT_MAX As Integer = 65535
		Private Const PRIV_PORT_MAX As Integer = 1023
		Private Const DEF_EPH_LOW As Integer = 49152

		' the actions mask
		<NonSerialized> _
		Private mask As Integer

		''' <summary>
		''' the actions string.
		''' 
		''' @serial
		''' </summary>

		Private actions As String ' Left null as long as possible, then
								' created and re-used in the getAction function.

		' hostname part as it is passed
		<NonSerialized> _
		Private hostname As String

		' the canonical name of the host
		' in the case of "*.foo.com", cname is ".foo.com".

		<NonSerialized> _
		Private cname As String

		' all the IP addresses of the host
		<NonSerialized> _
		Private addresses As java.net.InetAddress()

		' true if the hostname is a wildcard (e.g. "*.sun.com")
		<NonSerialized> _
		Private wildcard As Boolean

		' true if we were initialized with a single numeric IP address
		<NonSerialized> _
		Private init_with_ip As Boolean

		' true if this SocketPermission represents an invalid/unknown host
		' used for implies when the delayed lookup has already failed
		<NonSerialized> _
		Private invalid As Boolean

		' port range on host
		<NonSerialized> _
		Private portrange As Integer()

		<NonSerialized> _
		Private defaultDeny As Boolean = False

		' true if this SocketPermission represents a hostname
		' that failed our reverse mapping heuristic test
		<NonSerialized> _
		Private untrusted As Boolean
		<NonSerialized> _
		Private trusted As Boolean

		' true if the sun.net.trustNameService system property is set
		Private Shared trustNameService As Boolean

		Private Shared debug As sun.security.util.Debug = Nothing
		Private Shared debugInit As Boolean = False

		' lazy initializer
		Private Class EphemeralRange
			Friend Shared ReadOnly low As Integer = initEphemeralPorts("low", DEF_EPH_LOW)
			Friend Shared ReadOnly high As Integer = initEphemeralPorts("high", PORT_MAX)
		End Class

		Shared Sub New()
			Dim tmp As Boolean? = java.security.AccessController.doPrivileged(New sun.security.action.GetBooleanAction("sun.net.trustNameService"))
			trustNameService = tmp
		End Sub

		<MethodImpl(MethodImplOptions.Synchronized)> _
		Private Property Shared debug As sun.security.util.Debug
			Get
				If Not debugInit Then
					debug = sun.security.util.Debug.getInstance("access")
					debugInit = True
				End If
				Return debug
			End Get
		End Property

		''' <summary>
		''' Creates a new SocketPermission object with the specified actions.
		''' The host is expressed as a DNS name, or as a numerical IP address.
		''' Optionally, a port or a portrange may be supplied (separated
		''' from the DNS name or IP address by a colon).
		''' <p>
		''' To specify the local machine, use "localhost" as the <i>host</i>.
		''' Also note: An empty <i>host</i> String ("") is equivalent to "localhost".
		''' <p>
		''' The <i>actions</i> parameter contains a comma-separated list of the
		''' actions granted for the specified host (and port(s)). Possible actions are
		''' "connect", "listen", "accept", "resolve", or
		''' any combination of those. "resolve" is automatically added
		''' when any of the other three are specified.
		''' <p>
		''' Examples of SocketPermission instantiation are the following:
		''' <pre>
		'''    nr = new SocketPermission("www.catalog.com", "connect");
		'''    nr = new SocketPermission("www.sun.com:80", "connect");
		'''    nr = new SocketPermission("*.sun.com", "connect");
		'''    nr = new SocketPermission("*.edu", "resolve");
		'''    nr = new SocketPermission("204.160.241.0", "connect");
		'''    nr = new SocketPermission("localhost:1024-65535", "listen");
		'''    nr = new SocketPermission("204.160.241.0:1024-65535", "connect");
		''' </pre>
		''' </summary>
		''' <param name="host"> the hostname or IPaddress of the computer, optionally
		''' including a colon followed by a port or port range. </param>
		''' <param name="action"> the action string. </param>
		Public Sub New(ByVal host As String, ByVal action As String)
			MyBase.New(getHost(host))
			' name initialized to getHost(host); NPE detected in getHost()
			init(name, getMask(action))
		End Sub


		Friend Sub New(ByVal host As String, ByVal mask As Integer)
			MyBase.New(getHost(host))
			' name initialized to getHost(host); NPE detected in getHost()
			init(name, mask)
		End Sub

		Private Sub setDeny()
			defaultDeny = True
		End Sub

		Private Shared Function getHost(ByVal host As String) As String
			If host.Equals("") Then
				Return "localhost"
			Else
	'             IPv6 literal address used in this context should follow
	'             * the format specified in RFC 2732;
	'             * if not, we try to solve the unambiguous case
	'             
				Dim ind As Integer
				If host.Chars(0) <> "["c Then
					ind = host.IndexOf(":"c)
					If ind <> host.LastIndexOf(":"c) Then
	'                     More than one ":", meaning IPv6 address is not
	'                     * in RFC 2732 format;
	'                     * We will rectify user errors for all unambiguious cases
	'                     
						Dim st As New java.util.StringTokenizer(host, ":")
						Dim tokens As Integer = st.countTokens()
						If tokens = 9 Then
							' IPv6 address followed by port
							ind = host.LastIndexOf(":"c)
							host = "[" & host.Substring(0, ind) & "]" & host.Substring(ind)
						ElseIf tokens = 8 AndAlso host.IndexOf("::") = -1 Then
							' IPv6 address only, not followed by port
							host = "[" & host & "]"
						Else
							' could be ambiguous
							Throw New IllegalArgumentException("Ambiguous" & " hostport part")
						End If
					End If
				End If
				Return host
			End If
		End Function

		Private Function parsePort(ByVal port As String) As Integer()

			If port Is Nothing OrElse port.Equals("") OrElse port.Equals("*") Then Return New Integer() {PORT_MIN, PORT_MAX}

			Dim dash As Integer = port.IndexOf("-"c)

			If dash = -1 Then
				Dim p As Integer = Convert.ToInt32(port)
				Return New Integer() {p, p}
			Else
				Dim low As String = port.Substring(0, dash)
				Dim high As String = port.Substring(dash+1)
				Dim l, h As Integer

				If low.Equals("") Then
					l = PORT_MIN
				Else
					l = Convert.ToInt32(low)
				End If

				If high.Equals("") Then
					h = PORT_MAX
				Else
					h = Convert.ToInt32(high)
				End If
				If l < 0 OrElse h < 0 OrElse h<l Then Throw New IllegalArgumentException("invalid port range")

				Return New Integer() {l, h}
			End If
		End Function

		''' <summary>
		''' Returns true if the permission has specified zero
		''' as its value (or lower bound) signifying the ephemeral range
		''' </summary>
		Private Function includesEphemerals() As Boolean
			Return portrange(0) = 0
		End Function

		''' <summary>
		''' Initialize the SocketPermission object. We don't do any DNS lookups
		''' as this point, instead we hold off until the implies method is
		''' called.
		''' </summary>
		Private Sub init(ByVal host As String, ByVal mask As Integer)
			' Set the integer mask that represents the actions

			If (mask And ALL) <> mask Then Throw New IllegalArgumentException("invalid actions mask")

			' always OR in RESOLVE if we allow any of the others
			Me.mask = mask Or RESOLVE

			' Parse the host name.  A name has up to three components, the
			' hostname, a port number, or two numbers representing a port
			' range.   "www.sun.com:8080-9090" is a valid host name.

			' With IPv6 an address can be 2010:836B:4179::836B:4179
			' An IPv6 address needs to be enclose in []
			' For ex: [2010:836B:4179::836B:4179]:8080-9090
			' Refer to RFC 2732 for more information.

			Dim rb As Integer = 0
			Dim start As Integer = 0, [end] As Integer = 0
			Dim sep As Integer = -1
			Dim hostport As String = host
			If host.Chars(0) = "["c Then
				start = 1
				rb = host.IndexOf("]"c)
				If rb <> -1 Then
					host = host.Substring(start, rb - start)
				Else
					Throw New IllegalArgumentException("invalid host/port: " & host)
				End If
				sep = hostport.IndexOf(":"c, rb+1)
			Else
				start = 0
				sep = host.IndexOf(":"c, rb)
				[end] = sep
				If sep <> -1 Then host = host.Substring(start, [end] - start)
			End If

			If sep <> -1 Then
				Dim port As String = hostport.Substring(sep+1)
				Try
					portrange = parsePort(port)
				Catch e As Exception
					Throw New IllegalArgumentException("invalid port range: " & port)
				End Try
			Else
				portrange = New Integer() { PORT_MIN, PORT_MAX }
			End If

			hostname = host

			' is this a domain wildcard specification
			If host.LastIndexOf("*"c) > 0 Then
				Throw New IllegalArgumentException("invalid host wildcard specification")
			ElseIf host.StartsWith("*") Then
				wildcard = True
				If host.Equals("*") Then
					cname = ""
				ElseIf host.StartsWith("*.") Then
					cname = host.Substring(1).ToLower()
				Else
				  Throw New IllegalArgumentException("invalid host wildcard specification")
				End If
				Return
			Else
				If host.length() > 0 Then
					' see if we are being initialized with an IP address.
					Dim ch As Char = host.Chars(0)
					If ch = ":"c OrElse Character.digit(ch, 16) <> -1 Then
						Dim ip_Renamed As SByte() = sun.net.util.IPAddressUtil.textToNumericFormatV4(host)
						If ip_Renamed Is Nothing Then ip_Renamed = sun.net.util.IPAddressUtil.textToNumericFormatV6(host)
						If ip_Renamed IsNot Nothing Then
							Try
								addresses = New java.net.InetAddress() {java.net.InetAddress.getByAddress(ip_Renamed) }
								init_with_ip = True
							Catch uhe As UnknownHostException
								' this shouldn't happen
								invalid = True
							End Try
						End If
					End If
				End If
			End If
		End Sub

		''' <summary>
		''' Convert an action string to an integer actions mask.
		''' </summary>
		''' <param name="action"> the action string </param>
		''' <returns> the action mask </returns>
		Private Shared Function getMask(ByVal action As String) As Integer

			If action Is Nothing Then Throw New NullPointerException("action can't be null")

			If action.Equals("") Then Throw New IllegalArgumentException("action can't be empty")

			Dim mask_Renamed As Integer = NONE

			' Use object identity comparison against known-interned strings for
			' performance benefit (these values are used heavily within the JDK).
			If action = sun.security.util.SecurityConstants.SOCKET_RESOLVE_ACTION Then
				Return RESOLVE
			ElseIf action = sun.security.util.SecurityConstants.SOCKET_CONNECT_ACTION Then
				Return CONNECT
			ElseIf action = sun.security.util.SecurityConstants.SOCKET_LISTEN_ACTION Then
				Return LISTEN
			ElseIf action = sun.security.util.SecurityConstants.SOCKET_ACCEPT_ACTION Then
				Return ACCEPT
			ElseIf action = sun.security.util.SecurityConstants.SOCKET_CONNECT_ACCEPT_ACTION Then
				Return CONNECT Or ACCEPT
			End If

			Dim a As Char() = action.ToCharArray()

			Dim i As Integer = a.Length - 1
			If i < 0 Then Return mask_Renamed

			Do While i <> -1
				Dim c As Char

				' skip whitespace
				c = a(i)
				Do While (i<>-1) AndAlso (c = " "c OrElse c = ControlChars.Cr OrElse c = ControlChars.Lf OrElse c = ControlChars.FormFeed OrElse c = ControlChars.Tab)
					i -= 1
					c = a(i)
				Loop

				' check for the known strings
				Dim matchlen As Integer

				If i >= 6 AndAlso (a(i-6) = "c"c OrElse a(i-6) = "C"c) AndAlso (a(i-5) = "o"c OrElse a(i-5) = "O"c) AndAlso (a(i-4) = "n"c OrElse a(i-4) = "N"c) AndAlso (a(i-3) = "n"c OrElse a(i-3) = "N"c) AndAlso (a(i-2) = "e"c OrElse a(i-2) = "E"c) AndAlso (a(i-1) = "c"c OrElse a(i-1) = "C"c) AndAlso (a(i) = "t"c OrElse a(i) = "T"c) Then
					matchlen = 7
					mask_Renamed = mask_Renamed Or CONNECT

				ElseIf i >= 6 AndAlso (a(i-6) = "r"c OrElse a(i-6) = "R"c) AndAlso (a(i-5) = "e"c OrElse a(i-5) = "E"c) AndAlso (a(i-4) = "s"c OrElse a(i-4) = "S"c) AndAlso (a(i-3) = "o"c OrElse a(i-3) = "O"c) AndAlso (a(i-2) = "l"c OrElse a(i-2) = "L"c) AndAlso (a(i-1) = "v"c OrElse a(i-1) = "V"c) AndAlso (a(i) = "e"c OrElse a(i) = "E"c) Then
					matchlen = 7
					mask_Renamed = mask_Renamed Or RESOLVE

				ElseIf i >= 5 AndAlso (a(i-5) = "l"c OrElse a(i-5) = "L"c) AndAlso (a(i-4) = "i"c OrElse a(i-4) = "I"c) AndAlso (a(i-3) = "s"c OrElse a(i-3) = "S"c) AndAlso (a(i-2) = "t"c OrElse a(i-2) = "T"c) AndAlso (a(i-1) = "e"c OrElse a(i-1) = "E"c) AndAlso (a(i) = "n"c OrElse a(i) = "N"c) Then
					matchlen = 6
					mask_Renamed = mask_Renamed Or LISTEN

				ElseIf i >= 5 AndAlso (a(i-5) = "a"c OrElse a(i-5) = "A"c) AndAlso (a(i-4) = "c"c OrElse a(i-4) = "C"c) AndAlso (a(i-3) = "c"c OrElse a(i-3) = "C"c) AndAlso (a(i-2) = "e"c OrElse a(i-2) = "E"c) AndAlso (a(i-1) = "p"c OrElse a(i-1) = "P"c) AndAlso (a(i) = "t"c OrElse a(i) = "T"c) Then
					matchlen = 6
					mask_Renamed = mask_Renamed Or ACCEPT

				Else
					' parse error
					Throw New IllegalArgumentException("invalid permission: " & action)
				End If

				' make sure we didn't just match the tail of a word
				' like "ackbarfaccept".  Also, skip to the comma.
				Dim seencomma As Boolean = False
				Do While i >= matchlen AndAlso Not seencomma
					Select Case a(i-matchlen)
					Case ","c
						seencomma = True
					Case " "c, ControlChars.Cr, ControlChars.Lf, ControlChars.FormFeed, ControlChars.Tab
					Case Else
						Throw New IllegalArgumentException("invalid permission: " & action)
					End Select
					i -= 1
				Loop

				' point i at the location of the comma minus one (or -1).
				i -= matchlen
			Loop

			Return mask_Renamed
		End Function

		Private Property untrusted As Boolean
			Get
				If trusted Then Return False
				If invalid OrElse untrusted Then Return True
				Try
					If (Not trustNameService) AndAlso (defaultDeny OrElse sun.net.www.URLConnection.isProxiedHost(hostname)) Then
						If Me.cname Is Nothing Then Me.canonName
						If Not match(cname, hostname) Then
							' Last chance
							If Not authorized(hostname, addresses(0).address) Then
								untrusted = True
								Dim debug_Renamed As sun.security.util.Debug = debug
								If debug_Renamed IsNot Nothing AndAlso sun.security.util.Debug.isOn("failure") Then debug_Renamed.println("socket access restriction: proxied host " & "(" & addresses(0) & ")" & " does not match " & cname & " from reverse lookup")
								Return True
							End If
						End If
						trusted = True
					End If
				Catch uhe As UnknownHostException
					invalid = True
					Throw uhe
				End Try
				Return False
			End Get
		End Property

		''' <summary>
		''' attempt to get the fully qualified domain name
		''' 
		''' </summary>
		Friend Sub getCanonName()
			If cname IsNot Nothing OrElse invalid OrElse untrusted Then Return

			' attempt to get the canonical name

			Try
				' first get the IP addresses if we don't have them yet
				' this is because we need the IP address to then get
				' FQDN.
				If addresses Is Nothing Then iP

				' we have to do this check, otherwise we might not
				' get the fully qualified domain name
				If init_with_ip Then
					cname = addresses(0).getHostName(False).ToLower()
				Else
				 cname = java.net.InetAddress.getByName(addresses(0).hostAddress).getHostName(False).ToLower()
				End If
			Catch uhe As UnknownHostException
				invalid = True
				Throw uhe
			End Try
		End Sub

		<NonSerialized> _
		Private cdomain, hdomain As String

		Private Function match(ByVal cname As String, ByVal hname As String) As Boolean
			Dim a As String = cname.ToLower()
			Dim b As String = hname.ToLower()
			If a.StartsWith(b) AndAlso ((a.length() = b.length()) OrElse (a.Chars(b.length()) = "."c)) Then Return True
			If cdomain Is Nothing Then cdomain = sun.net.RegisteredDomain.getRegisteredDomain(a)
			If hdomain Is Nothing Then hdomain = sun.net.RegisteredDomain.getRegisteredDomain(b)

			Return cdomain.length() <> 0 AndAlso hdomain.length() <> 0 AndAlso cdomain.Equals(hdomain)
		End Function

		Private Function authorized(ByVal cname As String, ByVal addr As SByte()) As Boolean
			If addr.Length = 4 Then
				Return authorizedIPv4(cname, addr)
			ElseIf addr.Length = 16 Then
				Return authorizedIPv6(cname, addr)
			Else
				Return False
			End If
		End Function

		Private Function authorizedIPv4(ByVal cname As String, ByVal addr As SByte()) As Boolean
			Dim authHost As String = ""
			Dim auth As java.net.InetAddress

			Try
				authHost = "auth." & (addr(3) And &Hff) & "." & (addr(2) And &Hff) & "." & (addr(1) And &Hff) & "." & (addr(0) And &Hff) & ".in-addr.arpa"
				' Following check seems unnecessary
				' auth = InetAddress.getAllByName0(authHost, false)[0];
				authHost = hostname + AscW("."c) + authHost
				auth = java.net.InetAddress.getAllByName0(authHost, False)(0)
				If auth.Equals(java.net.InetAddress.getByAddress(addr)) Then Return True
				Dim debug_Renamed As sun.security.util.Debug = debug
				If debug_Renamed IsNot Nothing AndAlso sun.security.util.Debug.isOn("failure") Then debug_Renamed.println("socket access restriction: IP address of " & auth & " != " & java.net.InetAddress.getByAddress(addr))
			Catch uhe As UnknownHostException
				Dim debug_Renamed As sun.security.util.Debug = debug
				If debug_Renamed IsNot Nothing AndAlso sun.security.util.Debug.isOn("failure") Then debug_Renamed.println("socket access restriction: forward lookup failed for " & authHost)
			End Try
			Return False
		End Function

		Private Function authorizedIPv6(ByVal cname As String, ByVal addr As SByte()) As Boolean
			Dim authHost As String = ""
			Dim auth As java.net.InetAddress

			Try
				Dim sb As New StringBuffer(39)

				For i As Integer = 15 To 0 Step -1
					sb.append( java.lang.[Integer].toHexString(((addr(i)) And &Hf)))
					sb.append("."c)
					sb.append( java.lang.[Integer].toHexString(((addr(i) >> 4) And &Hf)))
					sb.append("."c)
				Next i
				authHost = "auth." & sb.ToString() & "IP6.ARPA"
				'auth = InetAddress.getAllByName0(authHost, false)[0];
				authHost = hostname + AscW("."c) + authHost
				auth = java.net.InetAddress.getAllByName0(authHost, False)(0)
				If auth.Equals(java.net.InetAddress.getByAddress(addr)) Then Return True
				Dim debug_Renamed As sun.security.util.Debug = debug
				If debug_Renamed IsNot Nothing AndAlso sun.security.util.Debug.isOn("failure") Then debug_Renamed.println("socket access restriction: IP address of " & auth & " != " & java.net.InetAddress.getByAddress(addr))
			Catch uhe As UnknownHostException
				Dim debug_Renamed As sun.security.util.Debug = debug
				If debug_Renamed IsNot Nothing AndAlso sun.security.util.Debug.isOn("failure") Then debug_Renamed.println("socket access restriction: forward lookup failed for " & authHost)
			End Try
			Return False
		End Function


		''' <summary>
		''' get IP addresses. Sets invalid to true if we can't get them.
		''' 
		''' </summary>
		Friend Sub getIP()
			If addresses IsNot Nothing OrElse wildcard OrElse invalid Then Return

			Try
				' now get all the IP addresses
				Dim host_Renamed As String
				If name.Chars(0) = "["c Then
					' Literal IPv6 address
					host_Renamed = name.Substring(1, name.IndexOf("]"c) - 1)
				Else
					Dim i As Integer = name.IndexOf(":")
					If i = -1 Then
						host_Renamed = name
					Else
						host_Renamed = name.Substring(0,i)
					End If
				End If

				addresses = New java.net.InetAddress() {java.net.InetAddress.getAllByName0(host_Renamed, False)(0)}

			Catch uhe As UnknownHostException
				invalid = True
				Throw uhe
			Catch iobe As IndexOutOfBoundsException
				invalid = True
				Throw New UnknownHostException(name)
			End Try
		End Sub

		''' <summary>
		''' Checks if this socket permission object "implies" the
		''' specified permission.
		''' <P>
		''' More specifically, this method first ensures that all of the following
		''' are true (and returns false if any of them are not):
		''' <ul>
		''' <li> <i>p</i> is an instanceof SocketPermission,
		''' <li> <i>p</i>'s actions are a proper subset of this
		''' object's actions, and
		''' <li> <i>p</i>'s port range is included in this port range. Note:
		''' port range is ignored when p only contains the action, 'resolve'.
		''' </ul>
		''' 
		''' Then {@code implies} checks each of the following, in order,
		''' and for each returns true if the stated condition is true:
		''' <ul>
		''' <li> If this object was initialized with a single IP address and one of <i>p</i>'s
		''' IP addresses is equal to this object's IP address.
		''' <li>If this object is a wildcard domain (such as *.sun.com), and
		''' <i>p</i>'s canonical name (the name without any preceding *)
		''' ends with this object's canonical host name. For example, *.sun.com
		''' implies *.eng.sun.com.
		''' <li>If this object was not initialized with a single IP address, and one of this
		''' object's IP addresses equals one of <i>p</i>'s IP addresses.
		''' <li>If this canonical name equals <i>p</i>'s canonical name.
		''' </ul>
		''' 
		''' If none of the above are true, {@code implies} returns false. </summary>
		''' <param name="p"> the permission to check against.
		''' </param>
		''' <returns> true if the specified permission is implied by this object,
		''' false if not. </returns>
		Public Function implies(ByVal p As java.security.Permission) As Boolean
			Dim i, j As Integer

			If Not(TypeOf p Is SocketPermission) Then Return False

			If p Is Me Then Return True

			Dim that As SocketPermission = CType(p, SocketPermission)

			Return ((Me.mask And that.mask) = that.mask) AndAlso impliesIgnoreMask(that)
		End Function

		''' <summary>
		''' Checks if the incoming Permission's action are a proper subset of
		''' the this object's actions.
		''' <P>
		''' Check, in the following order:
		''' <ul>
		''' <li> Checks that "p" is an instanceof a SocketPermission
		''' <li> Checks that "p"'s actions are a proper subset of the
		''' current object's actions.
		''' <li> Checks that "p"'s port range is included in this port range
		''' <li> If this object was initialized with an IP address, checks that
		'''      one of "p"'s IP addresses is equal to this object's IP address.
		''' <li> If either object is a wildcard domain (i.e., "*.sun.com"),
		'''      attempt to match based on the wildcard.
		''' <li> If this object was not initialized with an IP address, attempt
		'''      to find a match based on the IP addresses in both objects.
		''' <li> Attempt to match on the canonical hostnames of both objects.
		''' </ul> </summary>
		''' <param name="that"> the incoming permission request
		''' </param>
		''' <returns> true if "permission" is a proper subset of the current object,
		''' false if not. </returns>
		Friend Function impliesIgnoreMask(ByVal that As SocketPermission) As Boolean
			Dim i, j As Integer

			If (that.mask And RESOLVE) <> that.mask Then

				' check simple port range
				If (that.portrange(0) < Me.portrange(0)) OrElse (that.portrange(1) > Me.portrange(1)) Then

					' if either includes the ephemeral range, do full check
					If Me.includesEphemerals() OrElse that.includesEphemerals() Then
						If Not inRange(Me.portrange(0), Me.portrange(1), that.portrange(0), that.portrange(1)) Then Return False
					Else
						Return False
					End If
				End If
			End If

			' allow a "*" wildcard to always match anything
			If Me.wildcard AndAlso "".Equals(Me.cname) Then Return True

			' return if either one of these NetPerm objects are invalid...
			If Me.invalid OrElse that.invalid Then Return compareHostnames(that)

			Try
				If Me.init_with_ip Then ' we only check IP addresses
					If that.wildcard Then Return False

					If that.init_with_ip Then
						Return (Me.addresses(0).Equals(that.addresses(0)))
					Else
						If that.addresses Is Nothing Then that.iP
						For i = 0 To that.addresses.Length - 1
							If Me.addresses(0).Equals(that.addresses(i)) Then Return True
						Next i
					End If
					' since "this" was initialized with an IP address, we
					' don't check any other cases
					Return False
				End If

				' check and see if we have any wildcards...
				If Me.wildcard OrElse that.wildcard Then
					' if they are both wildcards, return true iff
					' that's cname ends with this cname (i.e., *.sun.com
					' implies *.eng.sun.com)
					If Me.wildcard AndAlso that.wildcard Then Return (that.cname.EndsWith(Me.cname))

					' a non-wildcard can't imply a wildcard
					If that.wildcard Then Return False

					' this is a wildcard, lets see if that's cname ends with
					' it...
					If that.cname Is Nothing Then that.canonName
					Return (that.cname.EndsWith(Me.cname))
				End If

				' comapare IP addresses
				If Me.addresses Is Nothing Then Me.iP

				If that.addresses Is Nothing Then that.iP

				If Not(that.init_with_ip AndAlso Me.untrusted) Then
					For j = 0 To Me.addresses.Length - 1
						For i = 0 To that.addresses.Length - 1
							If Me.addresses(j).Equals(that.addresses(i)) Then Return True
						Next i
					Next j

					' XXX: if all else fails, compare hostnames?
					' Do we really want this?
					If Me.cname Is Nothing Then Me.canonName

					If that.cname Is Nothing Then that.canonName

					Return (Me.cname.equalsIgnoreCase(that.cname))
				End If

			Catch uhe As UnknownHostException
				Return compareHostnames(that)
			End Try

			' make sure the first thing that is done here is to return
			' false. If not, uncomment the return false in the above catch.

			Return False
		End Function

		Private Function compareHostnames(ByVal that As SocketPermission) As Boolean
			' we see if the original names/IPs passed in were equal.

			Dim thisHost As String = hostname
			Dim thatHost As String = that.hostname

			If thisHost Is Nothing Then
				Return False
			ElseIf Me.wildcard Then
				Dim cnameLength As Integer = Me.cname.length()
				Return thatHost.regionMatches(True, (thatHost.length() - cnameLength), Me.cname, 0, cnameLength)
			Else
				Return thisHost.equalsIgnoreCase(thatHost)
			End If
		End Function

		''' <summary>
		''' Checks two SocketPermission objects for equality.
		''' <P> </summary>
		''' <param name="obj"> the object to test for equality with this object.
		''' </param>
		''' <returns> true if <i>obj</i> is a SocketPermission, and has the
		'''  same hostname, port range, and actions as this
		'''  SocketPermission object. However, port range will be ignored
		'''  in the comparison if <i>obj</i> only contains the action, 'resolve'. </returns>
		Public Overrides Function Equals(ByVal obj As Object) As Boolean
			If obj Is Me Then Return True

			If Not(TypeOf obj Is SocketPermission) Then Return False

			Dim that As SocketPermission = CType(obj, SocketPermission)

			'this is (overly?) complex!!!

			' check the mask first
			If Me.mask <> that.mask Then Return False

			If (that.mask And RESOLVE) <> that.mask Then
				' now check the port range...
				If (Me.portrange(0) <> that.portrange(0)) OrElse (Me.portrange(1) <> that.portrange(1)) Then Return False
			End If

			' short cut. This catches:
			'  "crypto" equal to "crypto", or
			' "1.2.3.4" equal to "1.2.3.4.", or
			'  "*.edu" equal to "*.edu", but it
			'  does not catch "crypto" equal to
			' "crypto.eng.sun.com".

			If Me.name.equalsIgnoreCase(that.name) Then Return True

			' we now attempt to get the Canonical (FQDN) name and
			' compare that. If this fails, about all we can do is return
			' false.

			Try
				Me.canonName
				that.canonName
			Catch uhe As UnknownHostException
				Return False
			End Try

			If Me.invalid OrElse that.invalid Then Return False

			If Me.cname IsNot Nothing Then Return Me.cname.equalsIgnoreCase(that.cname)

			Return False
		End Function

		''' <summary>
		''' Returns the hash code value for this object.
		''' </summary>
		''' <returns> a hash code value for this object. </returns>

		Public Overrides Function GetHashCode() As Integer
	'        
	'         * If this SocketPermission was initialized with an IP address
	'         * or a wildcard, use getName().hashCode(), otherwise use
	'         * the hashCode() of the host name returned from
	'         * java.net.InetAddress.getHostName method.
	'         

			If init_with_ip OrElse wildcard Then Return Me.name.GetHashCode()

			Try
				canonName
			Catch uhe As UnknownHostException

			End Try

			If invalid OrElse cname Is Nothing Then
				Return Me.name.GetHashCode()
			Else
				Return Me.cname.GetHashCode()
			End If
		End Function

		''' <summary>
		''' Return the current action mask.
		''' </summary>
		''' <returns> the actions mask. </returns>

		Friend Property mask As Integer
			Get
				Return mask
			End Get
		End Property

		''' <summary>
		''' Returns the "canonical string representation" of the actions in the
		''' specified mask.
		''' Always returns present actions in the following order:
		''' connect, listen, accept, resolve.
		''' </summary>
		''' <param name="mask"> a specific integer action mask to translate into a string </param>
		''' <returns> the canonical string representation of the actions </returns>
		Private Shared Function getActions(ByVal mask As Integer) As String
			Dim sb As New StringBuilder
			Dim comma As Boolean = False

			If (mask And CONNECT) = CONNECT Then
				comma = True
				sb.append("connect")
			End If

			If (mask And LISTEN) = LISTEN Then
				If comma Then
					sb.append(","c)
				Else
					comma = True
				End If
				sb.append("listen")
			End If

			If (mask And ACCEPT) = ACCEPT Then
				If comma Then
					sb.append(","c)
				Else
					comma = True
				End If
				sb.append("accept")
			End If


			If (mask And RESOLVE) = RESOLVE Then
				If comma Then
					sb.append(","c)
				Else
					comma = True
				End If
				sb.append("resolve")
			End If

			Return sb.ToString()
		End Function

		''' <summary>
		''' Returns the canonical string representation of the actions.
		''' Always returns present actions in the following order:
		''' connect, listen, accept, resolve.
		''' </summary>
		''' <returns> the canonical string representation of the actions. </returns>
		Public Property Overrides actions As String
			Get
				If actions Is Nothing Then actions = getActions(Me.mask)
    
				Return actions
			End Get
		End Property

		''' <summary>
		''' Returns a new PermissionCollection object for storing SocketPermission
		''' objects.
		''' <p>
		''' SocketPermission objects must be stored in a manner that allows them
		''' to be inserted into the collection in any order, but that also enables the
		''' PermissionCollection {@code implies}
		''' method to be implemented in an efficient (and consistent) manner.
		''' </summary>
		''' <returns> a new PermissionCollection object suitable for storing SocketPermissions. </returns>

		Public Overrides Function newPermissionCollection() As java.security.PermissionCollection
			Return New SocketPermissionCollection
		End Function

		''' <summary>
		''' WriteObject is called to save the state of the SocketPermission
		''' to a stream. The actions are serialized, and the superclass
		''' takes care of the name.
		''' </summary>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Private Sub writeObject(ByVal s As java.io.ObjectOutputStream)
			' Write out the actions. The superclass takes care of the name
			' call getActions to make sure actions field is initialized
			If actions Is Nothing Then actions
			s.defaultWriteObject()
		End Sub

		''' <summary>
		''' readObject is called to restore the state of the SocketPermission from
		''' a stream.
		''' </summary>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Private Sub readObject(ByVal s As java.io.ObjectInputStream)
			' Read in the action, then initialize the rest
			s.defaultReadObject()
			init(name,getMask(actions))
		End Sub

		''' <summary>
		''' Check the system/security property for the ephemeral port range
		''' for this system. The suffix is either "high" or "low"
		''' </summary>
		Private Shared Function initEphemeralPorts(ByVal suffix As String, ByVal defval As Integer) As Integer
			Return java.security.AccessController.doPrivileged(New PrivilegedActionAnonymousInnerClassHelper(Of T)
		   )
		End Function

		Private Class PrivilegedActionAnonymousInnerClassHelper(Of T)
			Implements java.security.PrivilegedAction(Of T)

			Public Overridable Function run() As Integer?
				Dim val As Integer =  java.lang.[Integer].getInteger("jdk.net.ephemeralPortRange." & suffix, -1)
				If val <> -1 Then
					Return val
				Else
					Return If(suffix.Equals("low"), sun.net.PortConfig.lower, sun.net.PortConfig.upper)
				End If
			End Function
		End Class

		''' <summary>
		''' Check if the target range is within the policy range
		''' together with the ephemeral range for this platform
		''' (if policy includes ephemeral range)
		''' </summary>
		Private Shared Function inRange(ByVal policyLow As Integer, ByVal policyHigh As Integer, ByVal targetLow As Integer, ByVal targetHigh As Integer) As Boolean
			Dim ephemeralLow As Integer = EphemeralRange.low
			Dim ephemeralHigh As Integer = EphemeralRange.high

			If targetLow = 0 Then
				' check policy includes ephemeral range
				If Not inRange(policyLow, policyHigh, ephemeralLow, ephemeralHigh) Then Return False
				If targetHigh = 0 Then Return True
				' continue check with first real port number
				targetLow = 1
			End If

			If policyLow = 0 AndAlso policyHigh = 0 Then Return targetLow >= ephemeralLow AndAlso targetHigh <= ephemeralHigh

			If policyLow <> 0 Then Return targetLow >= policyLow AndAlso targetHigh <= policyHigh

			' policyLow == 0 which means possibly two ranges to check

			' first check if policy and ephem range overlap/contiguous

			If policyHigh >= ephemeralLow - 1 Then Return targetHigh <= ephemeralHigh

			' policy and ephem range do not overlap

			' target range must lie entirely inside policy range or eph range

			Return (targetLow <= policyHigh AndAlso targetHigh <= policyHigh) OrElse (targetLow >= ephemeralLow AndAlso targetHigh <= ephemeralHigh)
		End Function
	'    
	'    public String toString()
	'    {
	'        StringBuffer s = new StringBuffer(super.toString() + "\n" +
	'            "cname = " + cname + "\n" +
	'            "wildcard = " + wildcard + "\n" +
	'            "invalid = " + invalid + "\n" +
	'            "portrange = " + portrange[0] + "," + portrange[1] + "\n");
	'        if (addresses != null) for (int i=0; i<addresses.length; i++) {
	'            s.append( addresses[i].getHostAddress());
	'            s.append("\n");
	'        } else {
	'            s.append("(no addresses)\n");
	'        }
	'
	'        return s.toString();
	'    }
	'
	'    public static  Sub  main(String args[]) throws Exception {
	'        SocketPermission this_ = new SocketPermission(args[0], "connect");
	'        SocketPermission that_ = new SocketPermission(args[1], "connect");
	'        System.out.println("-----\n");
	'        System.out.println("this.implies(that) = " + this_.implies(that_));
	'        System.out.println("-----\n");
	'        System.out.println("this = "+this_);
	'        System.out.println("-----\n");
	'        System.out.println("that = "+that_);
	'        System.out.println("-----\n");
	'
	'        SocketPermissionCollection nps = new SocketPermissionCollection();
	'        nps.add(this_);
	'        nps.add(new SocketPermission("www-leland.stanford.edu","connect"));
	'        nps.add(new SocketPermission("www-sun.com","connect"));
	'        System.out.println("nps.implies(that) = " + nps.implies(that_));
	'        System.out.println("-----\n");
	'    }
	'    
	End Class

	''' 
	''' <summary>
	''' if (init'd with IP, key is IP as string)
	''' if wildcard, its the wild card
	''' else its the cname?
	''' 
	''' </summary>
	''' <seealso cref= java.security.Permission </seealso>
	''' <seealso cref= java.security.Permissions </seealso>
	''' <seealso cref= java.security.PermissionCollection
	''' 
	''' 
	''' @author Roland Schemers
	''' 
	''' @serial include </seealso>

	<Serializable> _
	Friend NotInheritable Class SocketPermissionCollection
		Inherits java.security.PermissionCollection

		' Not serialized; see serialization section at end of class
		<NonSerialized> _
		Private perms As IList(Of SocketPermission)

		''' <summary>
		''' Create an empty SocketPermissions object.
		''' 
		''' </summary>

		Public Sub New()
			perms = New List(Of SocketPermission)
		End Sub

		''' <summary>
		''' Adds a permission to the SocketPermissions. The key for the hash is
		''' the name in the case of wildcards, or all the IP addresses.
		''' </summary>
		''' <param name="permission"> the Permission object to add.
		''' </param>
		''' <exception cref="IllegalArgumentException"> - if the permission is not a
		'''                                       SocketPermission
		''' </exception>
		''' <exception cref="SecurityException"> - if this SocketPermissionCollection object
		'''                                has been marked readonly </exception>
		Public Sub add(ByVal permission As java.security.Permission)
			If Not(TypeOf permission Is SocketPermission) Then Throw New IllegalArgumentException("invalid permission: " & permission)
			If [readOnly] Then Throw New SecurityException("attempt to add a Permission to a readonly PermissionCollection")

			' optimization to ensure perms most likely to be tested
			' show up early (4301064)
			SyncLock Me
				perms.Insert(0, CType(permission, SocketPermission))
			End SyncLock
		End Sub

		''' <summary>
		''' Check and see if this collection of permissions implies the permissions
		''' expressed in "permission".
		''' </summary>
		''' <param name="permission"> the Permission object to compare
		''' </param>
		''' <returns> true if "permission" is a proper subset of a permission in
		''' the collection, false if not. </returns>

		Public Function implies(ByVal permission As java.security.Permission) As Boolean
			If Not(TypeOf permission Is SocketPermission) Then Return False

			Dim np As SocketPermission = CType(permission, SocketPermission)

			Dim desired As Integer = np.mask
			Dim effective As Integer = 0
			Dim needed As Integer = desired

			SyncLock Me
				Dim len As Integer = perms.Count
				'System.out.println("implies "+np);
				For i As Integer = 0 To len - 1
					Dim x As SocketPermission = perms(i)
					'System.out.println("  trying "+x);
					If ((needed And x.mask) <> 0) AndAlso x.impliesIgnoreMask(np) Then
						effective = effective Or x.mask
						If (effective And desired) = desired Then Return True
						needed = (desired Xor effective)
					End If
				Next i
			End SyncLock
			Return False
		End Function

		''' <summary>
		''' Returns an enumeration of all the SocketPermission objects in the
		''' container.
		''' </summary>
		''' <returns> an enumeration of all the SocketPermission objects. </returns>

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Overrides Function elements() As System.Collections.IEnumerator(Of java.security.Permission)
			' Convert Iterator into Enumeration
			SyncLock Me
				Return java.util.Collections.enumeration(CType(CType(perms, IList), IList(Of java.security.Permission)))
			End SyncLock
		End Function

		Private Const serialVersionUID As Long = 2787186408602843674L

		' Need to maintain serialization interoperability with earlier releases,
		' which had the serializable field:

		'
		' The SocketPermissions for this set.
		' @serial
		'
		' private Vector permissions;

		''' <summary>
		''' @serialField permissions java.util.Vector
		'''     A list of the SocketPermissions for this set.
		''' </summary>
		Private Shared ReadOnly serialPersistentFields As java.io.ObjectStreamField() = { New java.io.ObjectStreamField("permissions", GetType(ArrayList)) }

		''' <summary>
		''' @serialData "permissions" field (a Vector containing the SocketPermissions).
		''' </summary>
	'    
	'     * Writes the contents of the perms field out as a Vector for
	'     * serialization compatibility with earlier releases.
	'     
		Private Sub writeObject(ByVal out As java.io.ObjectOutputStream)
			' Don't call out.defaultWriteObject()

			' Write out Vector
			Dim permissions As New List(Of SocketPermission)(perms.Count)

			SyncLock Me
				permissions.AddRange(perms)
			End SyncLock

			Dim pfields As java.io.ObjectOutputStream.PutField = out.putFields()
			pfields.put("permissions", permissions)
			out.writeFields()
		End Sub

	'    
	'     * Reads in a Vector of SocketPermissions and saves them in the perms field.
	'     
		Private Sub readObject(ByVal [in] As java.io.ObjectInputStream)
			' Don't call in.defaultReadObject()

			' Read in serialized fields
			Dim gfields As java.io.ObjectInputStream.GetField = [in].readFields()

			' Get the one we want
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Dim permissions As List(Of SocketPermission) = CType(gfields.get("permissions", Nothing), List(Of SocketPermission))
			perms = New List(Of SocketPermission)(permissions.Count)
			perms.AddRange(permissions)
		End Sub
	End Class

End Namespace