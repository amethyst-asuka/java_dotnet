Imports Microsoft.VisualBasic
Imports System
Imports System.Collections
Imports System.Text

'
' * Copyright (c) 2002, 2013, Oracle and/or its affiliates. All rights reserved.
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


Namespace javax.management.remote




	''' <summary>
	''' <p>The address of a JMX API connector server.  Instances of this class
	''' are immutable.</p>
	''' 
	''' <p>The address is an <em>Abstract Service URL</em> for SLP, as
	''' defined in RFC 2609 and amended by RFC 3111.  It must look like
	''' this:</p>
	''' 
	''' <blockquote>
	''' 
	''' <code>service:jmx:<em>protocol</em>:<em>sap</em></code>
	''' 
	''' </blockquote>
	''' 
	''' <p>Here, <code><em>protocol</em></code> is the transport
	''' protocol to be used to connect to the connector server.  It is
	''' a string of one or more ASCII characters, each of which is a
	''' letter, a digit, or one of the characters <code>+</code> or
	''' <code>-</code>.  The first character must be a letter.
	''' Uppercase letters are converted into lowercase ones.</p>
	''' 
	''' <p><code><em>sap</em></code> is the address at which the connector
	''' server is found.  This address uses a subset of the syntax defined
	''' by RFC 2609 for IP-based protocols.  It is a subset because the
	''' <code>user@host</code> syntax is not supported.</p>
	''' 
	''' <p>The other syntaxes defined by RFC 2609 are not currently
	''' supported by this class.</p>
	''' 
	''' <p>The supported syntax is:</p>
	''' 
	''' <blockquote>
	''' 
	''' <code>//<em>[host[</em>:<em>port]][url-path]</em></code>
	''' 
	''' </blockquote>
	''' 
	''' <p>Square brackets <code>[]</code> indicate optional parts of
	''' the address.  Not all protocols will recognize all optional
	''' parts.</p>
	''' 
	''' <p>The <code><em>host</em></code> is a host name, an IPv4 numeric
	''' host address, or an IPv6 numeric address enclosed in square
	''' brackets.</p>
	''' 
	''' <p>The <code><em>port</em></code> is a decimal port number.  0
	''' means a default or anonymous port, depending on the protocol.</p>
	''' 
	''' <p>The <code><em>host</em></code> and <code><em>port</em></code>
	''' can be omitted.  The <code><em>port</em></code> cannot be supplied
	''' without a <code><em>host</em></code>.</p>
	''' 
	''' <p>The <code><em>url-path</em></code>, if any, begins with a slash
	''' (<code>/</code>) or a semicolon (<code>;</code>) and continues to
	''' the end of the address.  It can contain attributes using the
	''' semicolon syntax specified in RFC 2609.  Those attributes are not
	''' parsed by this class and incorrect attribute syntax is not
	''' detected.</p>
	''' 
	''' <p>Although it is legal according to RFC 2609 to have a
	''' <code><em>url-path</em></code> that begins with a semicolon, not
	''' all implementations of SLP allow it, so it is recommended to avoid
	''' that syntax.</p>
	''' 
	''' <p>Case is not significant in the initial
	''' <code>service:jmx:<em>protocol</em></code> string or in the host
	''' part of the address.  Depending on the protocol, case can be
	''' significant in the <code><em>url-path</em></code>.</p>
	''' </summary>
	''' <seealso cref= <a
	''' href="http://www.ietf.org/rfc/rfc2609.txt">RFC 2609,
	''' "Service Templates and <code>Service:</code> Schemes"</a> </seealso>
	''' <seealso cref= <a
	''' href="http://www.ietf.org/rfc/rfc3111.txt">RFC 3111,
	''' "Service Location Protocol Modifications for IPv6"</a>
	''' 
	''' @since 1.5 </seealso>
	<Serializable> _
	Public Class JMXServiceURL

		Private Const serialVersionUID As Long = 8173364409860779292L

		''' <summary>
		''' <p>Constructs a <code>JMXServiceURL</code> by parsing a Service URL
		''' string.</p>
		''' </summary>
		''' <param name="serviceURL"> the URL string to be parsed.
		''' </param>
		''' <exception cref="NullPointerException"> if <code>serviceURL</code> is
		''' null.
		''' </exception>
		''' <exception cref="MalformedURLException"> if <code>serviceURL</code>
		''' does not conform to the syntax for an Abstract Service URL or
		''' if it is not a valid name for a JMX Remote API service.  A
		''' <code>JMXServiceURL</code> must begin with the string
		''' <code>"service:jmx:"</code> (case-insensitive).  It must not
		''' contain any characters that are not printable ASCII characters. </exception>
		Public Sub New(ByVal serviceURL As String)
			Dim serviceURLLength As Integer = serviceURL.Length

	'         Check that there are no non-ASCII characters in the URL,
	'           following RFC 2609.  
			For i As Integer = 0 To serviceURLLength - 1
				Dim c As Char = serviceURL.Chars(i)
				If AscW(c) < 32 OrElse c >= 127 Then Throw New java.net.MalformedURLException("Service URL contains " & "non-ASCII character 0x" & Integer.toHexString(c))
			Next i

			' Parse the required prefix
			Const requiredPrefix As String = "service:jmx:"
			Dim requiredPrefixLength As Integer = requiredPrefix.Length
			If Not serviceURL.regionMatches(True, 0, requiredPrefix, 0, requiredPrefixLength) Then ' requiredPrefix offset -  serviceURL offset -  ignore case Throw New java.net.MalformedURLException("Service URL must start with " & requiredPrefix)

			' Parse the protocol name
			Dim protoStart As Integer = requiredPrefixLength
			Dim protoEnd As Integer = IndexOf(serviceURL, ":"c, protoStart)
			Me.protocol = serviceURL.Substring(protoStart, protoEnd - protoStart).ToLower()

			If Not serviceURL.regionMatches(protoEnd, "://", 0, 3) Then
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'				throw New java.net.MalformedURLException("Missing "": "protocol name"); } final int hostStart = protoEnd + 3; final int hostEnd; if (hostStart < serviceURLLength && serviceURL.charAt(hostStart) == "["c)
	'		' Parse the host name
	'			{
	'			hostEnd = serviceURL.indexOf("]"c, hostStart) + 1;
	'			if (hostEnd == 0)
	'				throw New MalformedURLException("Bad host name: [ without ]");
	'			Me.host = serviceURL.substring(hostStart + 1, hostEnd - 1 - (hostStart + 1));
	'			if (!isNumericIPv6Address(Me.host))
	'			{
	'				throw New MalformedURLException("Address inside [...] must " + "be numeric IPv6 address");
	'			}
	'		}
			Else
				hostEnd = indexOfFirstNotInSet(serviceURL, hostNameBitSet, hostStart)
				Me.host = serviceURL.Substring(hostStart, hostEnd - hostStart)
			End If

			' Parse the port number
			Dim portEnd As Integer
			If hostEnd < serviceURLLength AndAlso serviceURL.Chars(hostEnd) = ":"c Then
				If Me.host.length() = 0 Then Throw New java.net.MalformedURLException("Cannot give port number " & "without host name")
				Dim portStart As Integer = hostEnd + 1
				portEnd = indexOfFirstNotInSet(serviceURL, numericBitSet, portStart)
				Dim portString As String = serviceURL.Substring(portStart, portEnd - portStart)
				Try
					Me.port = Convert.ToInt32(portString)
				Catch e As NumberFormatException
					Throw New java.net.MalformedURLException("Bad port number: """ & portString & """: " & e)
				End Try
			Else
				portEnd = hostEnd
				Me.port = 0
			End If

			' Parse the URL path
			Dim urlPathStart As Integer = portEnd
			If urlPathStart < serviceURLLength Then
				Me.urlPath = serviceURL.Substring(urlPathStart)
			Else
				Me.urlPath = ""
			End If

			validate()
			End If

		''' <summary>
		''' <p>Constructs a <code>JMXServiceURL</code> with the given protocol,
		''' host, and port.  This constructor is equivalent to
		''' {@link #JMXServiceURL(String, String, int, String)
		''' JMXServiceURL(protocol, host, port, null)}.</p>
		''' </summary>
		''' <param name="protocol"> the protocol part of the URL.  If null, defaults
		''' to <code>jmxmp</code>.
		''' </param>
		''' <param name="host"> the host part of the URL.  If null, defaults to the
		''' local host name, as determined by
		''' <code>InetAddress.getLocalHost().getHostName()</code>.  If it
		''' is a numeric IPv6 address, it can optionally be enclosed in
		''' square brackets <code>[]</code>.
		''' </param>
		''' <param name="port"> the port part of the URL.
		''' </param>
		''' <exception cref="MalformedURLException"> if one of the parts is
		''' syntactically incorrect, or if <code>host</code> is null and it
		''' is not possible to find the local host name, or if
		''' <code>port</code> is negative. </exception>
		public JMXServiceURL(String protocol, String host, Integer port) throws java.net.MalformedURLException
			Me.New(protocol, host, port, Nothing)

		''' <summary>
		''' <p>Constructs a <code>JMXServiceURL</code> with the given parts.
		''' </summary>
		''' <param name="protocol"> the protocol part of the URL.  If null, defaults
		''' to <code>jmxmp</code>.
		''' </param>
		''' <param name="host"> the host part of the URL.  If null, defaults to the
		''' local host name, as determined by
		''' <code>InetAddress.getLocalHost().getHostName()</code>.  If it
		''' is a numeric IPv6 address, it can optionally be enclosed in
		''' square brackets <code>[]</code>.
		''' </param>
		''' <param name="port"> the port part of the URL.
		''' </param>
		''' <param name="urlPath"> the URL path part of the URL.  If null, defaults to
		''' the empty string.
		''' </param>
		''' <exception cref="MalformedURLException"> if one of the parts is
		''' syntactically incorrect, or if <code>host</code> is null and it
		''' is not possible to find the local host name, or if
		''' <code>port</code> is negative. </exception>
		public JMXServiceURL(String protocol, String host, Integer port, String urlPath) throws java.net.MalformedURLException
			If protocol Is Nothing Then protocol = "jmxmp"

			If host Is Nothing Then
				Dim local As java.net.InetAddress
				Try
					local = java.net.InetAddress.localHost
				Catch e As java.net.UnknownHostException
					Throw New java.net.MalformedURLException("Local host name unknown: " & e)
				End Try

				host = local.hostName

	'             We might have a hostname that violates DNS naming
	'               rules, for example that contains an `_'.  While we
	'               could be strict and throw an exception, this is rather
	'               user-hostile.  Instead we use its numerical IP address.
	'               We can only reasonably do this for the host==null case.
	'               If we're given an explicit host name that is illegal we
	'               have to reject it.  (Bug 5057532.)  
				Try
					validateHost(host, port)
				Catch e As java.net.MalformedURLException
					If logger.fineOn() Then logger.fine("JMXServiceURL", "Replacing illegal local host name " & host & " with numeric IP address " & "(see RFC 1034)", e)
					host = local.hostAddress
	'                 Use the numeric address, which could be either IPv4
	'                   or IPv6.  validateHost will accept either.  
				End Try
			End If

			If host.StartsWith("[") Then
				If Not host.EndsWith("]") Then Throw New java.net.MalformedURLException("Host starts with [ but " & "does not end with ]")
				host = host.Substring(1, host.length() - 1 - 1)
				If Not isNumericIPv6Address(host) Then Throw New java.net.MalformedURLException("Address inside [...] must " & "be numeric IPv6 address")
				If host.StartsWith("[") Then Throw New java.net.MalformedURLException("More than one [[...]]")
			End If

			Me.protocol = protocol.ToLower()
			Me.host = host
			Me.port = port

			If urlPath Is Nothing Then urlPath = ""
			Me.urlPath = urlPath

			validate()

		private static final String INVALID_INSTANCE_MSG = "Trying to deserialize an invalid instance of JMXServiceURL"
		private void readObject(java.io.ObjectInputStream inputStream) throws java.io.IOException, ClassNotFoundException
			Dim gf As java.io.ObjectInputStream.GetField = inputStream.readFields()
			Dim h As String = CStr(gf.get("host", Nothing))
			Dim p As Integer = CInt(Fix(gf.get("port", -1)))
			Dim proto As String = CStr(gf.get("protocol", Nothing))
			Dim url As String = CStr(gf.get("urlPath", Nothing))

			If proto Is Nothing OrElse url Is Nothing OrElse h Is Nothing Then
				Dim sb As (New StringBuilder(INVALID_INSTANCE_MSG)).Append("["c)
				Dim empty As Boolean = True
				If proto Is Nothing Then
					sb.Append("protocol=null")
					empty = False
				End If
				If h Is Nothing Then
					sb.Append(If(empty, "", ",")).append("host=null")
					empty = False
				End If
				If url Is Nothing Then sb.Append(If(empty, "", ",")).append("urlPath=null")
				sb.Append("]"c)
				Throw New java.io.InvalidObjectException(sb.ToString())
			End If

			If h.Contains("[") OrElse h.Contains("]") Then Throw New java.io.InvalidObjectException("Invalid host name: " & h)

			Try
				validate(proto, h, p, url)
				Me.protocol = proto
				Me.host = h
				Me.port = p
				Me.urlPath = url
			Catch e As java.net.MalformedURLException
				Throw New java.io.InvalidObjectException(INVALID_INSTANCE_MSG & ": " & e.Message)
			End Try


		private void validate(String proto, String h, Integer p, String url) throws java.net.MalformedURLException
			' Check protocol
			Dim protoEnd As Integer = indexOfFirstNotInSet(proto, protocolBitSet, 0)
			If protoEnd = 0 OrElse protoEnd < proto.length() OrElse (Not alphaBitSet.get(proto.Chars(0))) Then Throw New java.net.MalformedURLException("Missing or invalid protocol " & "name: """ & proto & """")

			' Check host
			validateHost(h, p)

			' Check port
			If p < 0 Then Throw New java.net.MalformedURLException("Bad port: " & p)

			' Check URL path
			If url.length() > 0 Then
				If (Not url.StartsWith("/")) AndAlso (Not url.StartsWith(";")) Then Throw New java.net.MalformedURLException("Bad URL path: " & url)
			End If

		private void validate() throws java.net.MalformedURLException
			validate(Me.protocol, Me.host, Me.port, Me.urlPath)

		private static void validateHost(String h, Integer port) throws java.net.MalformedURLException

			If h.length() = 0 Then
				If port <> 0 Then Throw New java.net.MalformedURLException("Cannot give port number " & "without host name")
				Return
			End If

			If isNumericIPv6Address(h) Then
	'             We assume J2SE >= 1.4 here.  Otherwise you can't
	'               use the address anyway.  We can't call
	'               InetAddress.getByName without checking for a
	'               numeric IPv6 address, because we mustn't try to do
	'               a DNS lookup in case the address is not actually
	'               numeric.  
				Try
					java.net.InetAddress.getByName(h)
				Catch e As Exception
	'                 We should really catch UnknownHostException
	'                   here, but a bug in JDK 1.4 causes it to throw
	'                   ArrayIndexOutOfBoundsException, e.g. if the
	'                   string is ":".  
					Dim bad As New java.net.MalformedURLException("Bad IPv6 address: " & h)
					com.sun.jmx.remote.util.EnvHelp.initCause(bad, e)
					Throw bad
				End Try
			Else
	'             Tiny state machine to check valid host name.  This
	'               checks the hostname grammar from RFC 1034 (DNS),
	'               page 11.  A hostname is a dot-separated list of one
	'               or more labels, where each label consists of
	'               letters, numbers, or hyphens.  A label cannot begin
	'               or end with a hyphen.  Empty hostnames are not
	'               allowed.  Note that numeric IPv4 addresses are a
	'               special case of this grammar.
	'
	'               The state is entirely captured by the last
	'               character seen, with a virtual `.' preceding the
	'               name.  We represent any alphanumeric character by
	'               `a'.
	'
	'               We need a special hack to check, as required by the
	'               RFC 2609 (SLP) grammar, that the last component of
	'               the hostname begins with a letter.  Respecting the
	'               intent of the RFC, we only do this if there is more
	'               than one component.  If your local hostname begins
	'               with a digit, we don't reject it.  
				Dim hostLen As Integer = h.length()
				Dim lastc As Char = "."c
				Dim sawDot As Boolean = False
				Dim componentStart As Char = 0

				loop:
				For i As Integer = 0 To hostLen - 1
					Dim c As Char = h.Chars(i)
					Dim isAlphaNumeric As Boolean = alphaNumericBitSet.get(c)
					If lastc = "."c Then componentStart = c
					If isAlphaNumeric Then
						lastc = "a"c
					ElseIf c = "-"c Then
						If lastc = "."c Then Exit For ' will throw exception
						lastc = "-"c
					ElseIf c = "."c Then
						sawDot = True
						If lastc <> "a"c Then Exit For ' will throw exception
						lastc = "."c
					Else
						lastc = "."c ' will throw exception
						Exit For
					End If
				Next i

				Try
					If lastc <> "a"c Then Throw randomException
					If sawDot AndAlso (Not alphaBitSet.get(componentStart)) Then
	'                     Must be a numeric IPv4 address.  In addition to
	'                       the explicitly-thrown exceptions, we can get
	'                       NoSuchElementException from the calls to
	'                       tok.nextToken and NumberFormatException from
	'                       the call to Integer.parseInt.  Using exceptions
	'                       for control flow this way is a bit evil but it
	'                       does simplify things enormously.  
						Dim tok As New java.util.StringTokenizer(h, ".", True)
						For i As Integer = 0 To 3
							Dim ns As String = tok.nextToken()
							Dim n As Integer = Convert.ToInt32(ns)
							If n < 0 OrElse n > 255 Then Throw randomException
							If i < 3 AndAlso (Not tok.nextToken().Equals(".")) Then Throw randomException
						Next i
						If tok.hasMoreTokens() Then Throw randomException
					End If
				Catch e As Exception
					Throw New java.net.MalformedURLException("Bad host: """ & h & """")
				End Try
			End If

		private static final Exception randomException = New Exception


		''' <summary>
		''' <p>The protocol part of the Service URL.
		''' </summary>
		''' <returns> the protocol part of the Service URL.  This is never null. </returns>
		public String protocol
			Return protocol

		''' <summary>
		''' <p>The host part of the Service URL.  If the Service URL was
		''' constructed with the constructor that takes a URL string
		''' parameter, the result is the substring specifying the host in
		''' that URL.  If the Service URL was constructed with a
		''' constructor that takes a separate host parameter, the result is
		''' the string that was specified.  If that string was null, the
		''' result is
		''' <code>InetAddress.getLocalHost().getHostName()</code>.</p>
		''' 
		''' <p>In either case, if the host was specified using the
		''' <code>[...]</code> syntax for numeric IPv6 addresses, the
		''' square brackets are not included in the return value here.</p>
		''' </summary>
		''' <returns> the host part of the Service URL.  This is never null. </returns>
		public String host
			Return host

		''' <summary>
		''' <p>The port of the Service URL.  If no port was
		''' specified, the returned value is 0.</p>
		''' </summary>
		''' <returns> the port of the Service URL, or 0 if none. </returns>
		public Integer port
			Return port

		''' <summary>
		''' <p>The URL Path part of the Service URL.  This is an empty
		''' string, or a string beginning with a slash (<code>/</code>), or
		''' a string beginning with a semicolon (<code>;</code>).
		''' </summary>
		''' <returns> the URL Path part of the Service URL.  This is never
		''' null. </returns>
		public String uRLPath
			Return urlPath

		''' <summary>
		''' <p>The string representation of this Service URL.  If the value
		''' returned by this method is supplied to the
		''' <code>JMXServiceURL</code> constructor, the resultant object is
		''' equal to this one.</p>
		''' 
		''' <p>The <code><em>host</em></code> part of the returned string
		''' is the value returned by <seealso cref="#getHost()"/>.  If that value
		''' specifies a numeric IPv6 address, it is surrounded by square
		''' brackets <code>[]</code>.</p>
		''' 
		''' <p>The <code><em>port</em></code> part of the returned string
		''' is the value returned by <seealso cref="#getPort()"/> in its shortest
		''' decimal form.  If the value is zero, it is omitted.</p>
		''' </summary>
		''' <returns> the string representation of this Service URL. </returns>
		public String ToString()
	'         We don't bother synchronizing the access to toString.  At worst,
	'           n threads will independently compute and store the same value.  
			If toString IsNot Nothing Then Return toString
			Dim buf As New StringBuilder("service:jmx:")
			buf.Append(protocol).append("://")
			Dim getHost As String = host
			If isNumericIPv6Address(getHost) Then
				buf.Append("["c).append(getHost).append("]"c)
			Else
				buf.Append(getHost)
			End If
			Dim getPort As Integer = port
			If getPort <> 0 Then buf.Append(":"c).append(getPort)
			buf.Append(uRLPath)
			toString = buf.ToString()
			Return toString

		''' <summary>
		''' <p>Indicates whether some other object is equal to this one.
		''' This method returns true if and only if <code>obj</code> is an
		''' instance of <code>JMXServiceURL</code> whose {@link
		''' #getProtocol()}, <seealso cref="#getHost()"/>, <seealso cref="#getPort()"/>, and
		''' <seealso cref="#getURLPath()"/> methods return the same values as for
		''' this object.  The values for <seealso cref="#getProtocol()"/> and {@link
		''' #getHost()} can differ in case without affecting equality.
		''' </summary>
		''' <param name="obj"> the reference object with which to compare.
		''' </param>
		''' <returns> <code>true</code> if this object is the same as the
		''' <code>obj</code> argument; <code>false</code> otherwise. </returns>
		public Boolean Equals(Object obj)
			If Not(TypeOf obj Is JMXServiceURL) Then Return False
			Dim u As JMXServiceURL = CType(obj, JMXServiceURL)
			Return (u.protocol.equalsIgnoreCase(protocol) AndAlso u.host.equalsIgnoreCase(host) AndAlso u.port = port AndAlso u.uRLPath.Equals(uRLPath))

		public Integer GetHashCode()
			Return ToString().GetHashCode()

	'     True if this string, assumed to be a valid argument to
	'     * InetAddress.getByName, is a numeric IPv6 address.
	'     
		private static Boolean isNumericIPv6Address(String s)
			' address contains colon if and only if it's a numeric IPv6 address
			Return (s.IndexOf(":"c) >= 0)

		' like String.indexOf but returns string length not -1 if not present
		private static Integer IndexOf(String s, Char c, Integer fromIndex)
			Dim index As Integer = s.IndexOf(c, fromIndex)
			If index < 0 Then
				Return s.length()
			Else
				Return index
			End If

		private static Integer indexOfFirstNotInSet(String s, BitArray set, Integer fromIndex)
			Dim slen As Integer = s.length()
			Dim i As Integer = fromIndex
			Do
				If i >= slen Then Exit Do
				Dim c As Char = s.Chars(i)
				If c >= 128 Then Exit Do ' not ASCII
				If Not set.get(c) Then Exit Do
				i += 1
			Loop
			Return i

		private final static BitArray alphaBitSet = New BitArray(128)
		private final static BitArray numericBitSet = New BitArray(128)
		private final static BitArray alphaNumericBitSet = New BitArray(128)
		private final static BitArray protocolBitSet = New BitArray(128)
		private final static BitArray hostNameBitSet = New BitArray(128)
		static JMXServiceURL()
	'         J2SE 1.4 adds lots of handy methods to BitSet that would
	'           allow us to simplify here, e.g. by not writing loops, but
	'           we want to work on J2SE 1.3 too.  

			For c As Char = AscW("0"c) To AscW("9"c)
				numericBitSet.set(c)
			Next c

			For c As Char = AscW("A"c) To AscW("Z"c)
				alphaBitSet.set(c)
			Next c
			For c As Char = AscW("a"c) To AscW("z"c)
				alphaBitSet.set(c)
			Next c

			alphaNumericBitSet.or(alphaBitSet)
			alphaNumericBitSet.or(numericBitSet)

			protocolBitSet.or(alphaNumericBitSet)
			protocolBitSet.set("+"c)
			protocolBitSet.set("-"c)

			hostNameBitSet.or(alphaNumericBitSet)
			hostNameBitSet.set("-"c)
			hostNameBitSet.set("."c)

		''' <summary>
		''' The value returned by <seealso cref="#getProtocol()"/>.
		''' </summary>
		private String protocol

		''' <summary>
		''' The value returned by <seealso cref="#getHost()"/>.
		''' </summary>
		private String host

		''' <summary>
		''' The value returned by <seealso cref="#getPort()"/>.
		''' </summary>
		private Integer port

		''' <summary>
		''' The value returned by <seealso cref="#getURLPath()"/>.
		''' </summary>
		private String urlPath

		''' <summary>
		''' Cached result of <seealso cref="#toString()"/>.
		''' </summary>
		private transient String toString

		private static final com.sun.jmx.remote.util.ClassLogger logger = New com.sun.jmx.remote.util.ClassLogger("javax.management.remote.misc", "JMXServiceURL")
		End Sub

End Namespace