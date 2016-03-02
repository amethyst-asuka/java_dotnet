Imports System
Imports System.Runtime.CompilerServices
Imports System.Collections.Generic

'
' * Copyright (c) 1995, 2015, Oracle and/or its affiliates. All rights reserved.
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
	''' Class {@code URL} represents a Uniform Resource
	''' Locator, a pointer to a "resource" on the World
	''' Wide Web. A resource can be something as simple as a file or a
	''' directory, or it can be a reference to a more complicated object,
	''' such as a query to a database or to a search engine. More
	''' information on the types of URLs and their formats can be found at:
	''' <a href=
	''' "http://web.archive.org/web/20051219043731/http://archive.ncsa.uiuc.edu/SDG/Software/Mosaic/Demo/url-primer.html">
	''' <i>Types of URL</i></a>
	''' <p>
	''' In general, a URL can be broken into several parts. Consider the
	''' following example:
	''' <blockquote><pre>
	'''     http://www.example.com/docs/resource1.html
	''' </pre></blockquote>
	''' <p>
	''' The URL above indicates that the protocol to use is
	''' {@code http} (HyperText Transfer Protocol) and that the
	''' information resides on a host machine named
	''' {@code www.example.com}. The information on that host
	''' machine is named {@code /docs/resource1.html}. The exact
	''' meaning of this name on the host machine is both protocol
	''' dependent and host dependent. The information normally resides in
	''' a file, but it could be generated on the fly. This component of
	''' the URL is called the <i>path</i> component.
	''' <p>
	''' A URL can optionally specify a "port", which is the
	''' port number to which the TCP connection is made on the remote host
	''' machine. If the port is not specified, the default port for
	''' the protocol is used instead. For example, the default port for
	''' {@code http} is {@code 80}. An alternative port could be
	''' specified as:
	''' <blockquote><pre>
	'''     http://www.example.com:1080/docs/resource1.html
	''' </pre></blockquote>
	''' <p>
	''' The syntax of {@code URL} is defined by  <a
	''' href="http://www.ietf.org/rfc/rfc2396.txt"><i>RFC&nbsp;2396: Uniform
	''' Resource Identifiers (URI): Generic Syntax</i></a>, amended by <a
	''' href="http://www.ietf.org/rfc/rfc2732.txt"><i>RFC&nbsp;2732: Format for
	''' Literal IPv6 Addresses in URLs</i></a>. The Literal IPv6 address format
	''' also supports scope_ids. The syntax and usage of scope_ids is described
	''' <a href="Inet6Address.html#scoped">here</a>.
	''' <p>
	''' A URL may have appended to it a "fragment", also known
	''' as a "ref" or a "reference". The fragment is indicated by the sharp
	''' sign character "#" followed by more characters. For example,
	''' <blockquote><pre>
	'''     http://java.sun.com/index.html#chapter1
	''' </pre></blockquote>
	''' <p>
	''' This fragment is not technically part of the URL. Rather, it
	''' indicates that after the specified resource is retrieved, the
	''' application is specifically interested in that part of the
	''' document that has the tag {@code chapter1} attached to it. The
	''' meaning of a tag is resource specific.
	''' <p>
	''' An application can also specify a "relative URL",
	''' which contains only enough information to reach the resource
	''' relative to another URL. Relative URLs are frequently used within
	''' HTML pages. For example, if the contents of the URL:
	''' <blockquote><pre>
	'''     http://java.sun.com/index.html
	''' </pre></blockquote>
	''' contained within it the relative URL:
	''' <blockquote><pre>
	'''     FAQ.html
	''' </pre></blockquote>
	''' it would be a shorthand for:
	''' <blockquote><pre>
	'''     http://java.sun.com/FAQ.html
	''' </pre></blockquote>
	''' <p>
	''' The relative URL need not specify all the components of a URL. If
	''' the protocol, host name, or port number is missing, the value is
	''' inherited from the fully specified URL. The file component must be
	''' specified. The optional fragment is not inherited.
	''' <p>
	''' The URL class does not itself encode or decode any URL components
	''' according to the escaping mechanism defined in RFC2396. It is the
	''' responsibility of the caller to encode any fields, which need to be
	''' escaped prior to calling URL, and also to decode any escaped fields,
	''' that are returned from URL. Furthermore, because URL has no knowledge
	''' of URL escaping, it does not recognise equivalence between the encoded
	''' or decoded form of the same URL. For example, the two URLs:<br>
	''' <pre>    http://foo.com/hello world/ and http://foo.com/hello%20world</pre>
	''' would be considered not equal to each other.
	''' <p>
	''' Note, the <seealso cref="java.net.URI"/> class does perform escaping of its
	''' component fields in certain circumstances. The recommended way
	''' to manage the encoding and decoding of URLs is to use <seealso cref="java.net.URI"/>,
	''' and to convert between these two classes using <seealso cref="#toURI()"/> and
	''' <seealso cref="URI#toURL()"/>.
	''' <p>
	''' The <seealso cref="URLEncoder"/> and <seealso cref="URLDecoder"/> classes can also be
	''' used, but only for HTML form encoding, which is not the same
	''' as the encoding scheme defined in RFC2396.
	''' 
	''' @author  James Gosling
	''' @since JDK1.0
	''' </summary>
	<Serializable> _
	Public NotInheritable Class URL

		Friend Const BUILTIN_HANDLERS_PREFIX As String = "sun.net.www.protocol"
		Friend Const serialVersionUID As Long = -7627629688361524110L

		''' <summary>
		''' The property which specifies the package prefix list to be scanned
		''' for protocol handlers.  The value of this property (if any) should
		''' be a vertical bar delimited list of package names to search through
		''' for a protocol handler to load.  The policy of this class is that
		''' all protocol handlers will be in a class called <protocolname>.Handler,
		''' and each package in the list is examined in turn for a matching
		''' handler.  If none are found (or the property is not specified), the
		''' default package prefix, sun.net.www.protocol, is used.  The search
		''' proceeds from the first package in the list to the last and stops
		''' when a match is found.
		''' </summary>
		Private Const protocolPathProp As String = "java.protocol.handler.pkgs"

		''' <summary>
		''' The protocol to use (ftp, http, nntp, ... etc.) .
		''' @serial
		''' </summary>
		Private protocol As String

		''' <summary>
		''' The host name to connect to.
		''' @serial
		''' </summary>
		Private host As String

		''' <summary>
		''' The protocol port to connect to.
		''' @serial
		''' </summary>
		Private port As Integer = -1

		''' <summary>
		''' The specified file name on that host. {@code file} is
		''' defined as {@code path[?query]}
		''' @serial
		''' </summary>
		Private file As String

		''' <summary>
		''' The query part of this URL.
		''' </summary>
		<NonSerialized> _
		Private query As String

		''' <summary>
		''' The authority part of this URL.
		''' @serial
		''' </summary>
		Private authority As String

		''' <summary>
		''' The path part of this URL.
		''' </summary>
		<NonSerialized> _
		Private path As String

		''' <summary>
		''' The userinfo part of this URL.
		''' </summary>
		<NonSerialized> _
		Private userInfo As String

		''' <summary>
		''' # reference.
		''' @serial
		''' </summary>
		Private ref As String

		''' <summary>
		''' The host's IP address, used in equals and hashCode.
		''' Computed on demand. An uninitialized or unknown hostAddress is null.
		''' </summary>
		<NonSerialized> _
		Friend hostAddress As InetAddress

		''' <summary>
		''' The URLStreamHandler for this URL.
		''' </summary>
		<NonSerialized> _
		Friend handler As URLStreamHandler

	'     Our hash code.
	'     * @serial
	'     
		Private hashCode_Renamed As Integer = -1

		<NonSerialized> _
		Private tempState As UrlDeserializedState

		''' <summary>
		''' Creates a {@code URL} object from the specified
		''' {@code protocol}, {@code host}, {@code port}
		''' number, and {@code file}.<p>
		''' 
		''' {@code host} can be expressed as a host name or a literal
		''' IP address. If IPv6 literal address is used, it should be
		''' enclosed in square brackets ({@code '['} and {@code ']'}), as
		''' specified by <a
		''' href="http://www.ietf.org/rfc/rfc2732.txt">RFC&nbsp;2732</a>;
		''' However, the literal IPv6 address format defined in <a
		''' href="http://www.ietf.org/rfc/rfc2373.txt"><i>RFC&nbsp;2373: IP
		''' Version 6 Addressing Architecture</i></a> is also accepted.<p>
		''' 
		''' Specifying a {@code port} number of {@code -1}
		''' indicates that the URL should use the default port for the
		''' protocol.<p>
		''' 
		''' If this is the first URL object being created with the specified
		''' protocol, a <i>stream protocol handler</i> object, an instance of
		''' class {@code URLStreamHandler}, is created for that protocol:
		''' <ol>
		''' <li>If the application has previously set up an instance of
		'''     {@code URLStreamHandlerFactory} as the stream handler factory,
		'''     then the {@code createURLStreamHandler} method of that instance
		'''     is called with the protocol string as an argument to create the
		'''     stream protocol handler.
		''' <li>If no {@code URLStreamHandlerFactory} has yet been set up,
		'''     or if the factory's {@code createURLStreamHandler} method
		'''     returns {@code null}, then the constructor finds the
		'''     value of the system property:
		'''     <blockquote><pre>
		'''         java.protocol.handler.pkgs
		'''     </pre></blockquote>
		'''     If the value of that system property is not {@code null},
		'''     it is interpreted as a list of packages separated by a vertical
		'''     slash character '{@code |}'. The constructor tries to load
		'''     the class named:
		'''     <blockquote><pre>
		'''         &lt;<i>package</i>&gt;.&lt;<i>protocol</i>&gt;.Handler
		'''     </pre></blockquote>
		'''     where &lt;<i>package</i>&gt; is replaced by the name of the package
		'''     and &lt;<i>protocol</i>&gt; is replaced by the name of the protocol.
		'''     If this class does not exist, or if the class exists but it is not
		'''     a subclass of {@code URLStreamHandler}, then the next package
		'''     in the list is tried.
		''' <li>If the previous step fails to find a protocol handler, then the
		'''     constructor tries to load from a system default package.
		'''     <blockquote><pre>
		'''         &lt;<i>system default package</i>&gt;.&lt;<i>protocol</i>&gt;.Handler
		'''     </pre></blockquote>
		'''     If this class does not exist, or if the class exists but it is not a
		'''     subclass of {@code URLStreamHandler}, then a
		'''     {@code MalformedURLException} is thrown.
		''' </ol>
		''' 
		''' <p>Protocol handlers for the following protocols are guaranteed
		''' to exist on the search path :-
		''' <blockquote><pre>
		'''     http, https, file, and jar
		''' </pre></blockquote>
		''' Protocol handlers for additional protocols may also be
		''' available.
		''' 
		''' <p>No validation of the inputs is performed by this constructor.
		''' </summary>
		''' <param name="protocol">   the name of the protocol to use. </param>
		''' <param name="host">       the name of the host. </param>
		''' <param name="port">       the port number on the host. </param>
		''' <param name="file">       the file on the host </param>
		''' <exception cref="MalformedURLException">  if an unknown protocol is specified. </exception>
		''' <seealso cref=        java.lang.System#getProperty(java.lang.String) </seealso>
		''' <seealso cref=        java.net.URL#setURLStreamHandlerFactory(
		'''                  java.net.URLStreamHandlerFactory) </seealso>
		''' <seealso cref=        java.net.URLStreamHandler </seealso>
		''' <seealso cref=        java.net.URLStreamHandlerFactory#createURLStreamHandler(
		'''                  java.lang.String) </seealso>
		Public Sub New(ByVal protocol As String, ByVal host As String, ByVal port As Integer, ByVal file As String)
			Me.New(protocol, host, port, file, Nothing)
		End Sub

		''' <summary>
		''' Creates a URL from the specified {@code protocol}
		''' name, {@code host} name, and {@code file} name. The
		''' default port for the specified protocol is used.
		''' <p>
		''' This method is equivalent to calling the four-argument
		''' constructor with the arguments being {@code protocol},
		''' {@code host}, {@code -1}, and {@code file}.
		''' 
		''' No validation of the inputs is performed by this constructor.
		''' </summary>
		''' <param name="protocol">   the name of the protocol to use. </param>
		''' <param name="host">       the name of the host. </param>
		''' <param name="file">       the file on the host. </param>
		''' <exception cref="MalformedURLException">  if an unknown protocol is specified. </exception>
		''' <seealso cref=        java.net.URL#URL(java.lang.String, java.lang.String,
		'''                  int, java.lang.String) </seealso>
		Public Sub New(ByVal protocol As String, ByVal host As String, ByVal file As String)
			Me.New(protocol, host, -1, file)
		End Sub

		''' <summary>
		''' Creates a {@code URL} object from the specified
		''' {@code protocol}, {@code host}, {@code port}
		''' number, {@code file}, and {@code handler}. Specifying
		''' a {@code port} number of {@code -1} indicates that
		''' the URL should use the default port for the protocol. Specifying
		''' a {@code handler} of {@code null} indicates that the URL
		''' should use a default stream handler for the protocol, as outlined
		''' for:
		'''     java.net.URL#URL(java.lang.String, java.lang.String, int,
		'''                      java.lang.String)
		''' 
		''' <p>If the handler is not null and there is a security manager,
		''' the security manager's {@code checkPermission}
		''' method is called with a
		''' {@code NetPermission("specifyStreamHandler")} permission.
		''' This may result in a SecurityException.
		''' 
		''' No validation of the inputs is performed by this constructor.
		''' </summary>
		''' <param name="protocol">   the name of the protocol to use. </param>
		''' <param name="host">       the name of the host. </param>
		''' <param name="port">       the port number on the host. </param>
		''' <param name="file">       the file on the host </param>
		''' <param name="handler">    the stream handler for the URL. </param>
		''' <exception cref="MalformedURLException">  if an unknown protocol is specified. </exception>
		''' <exception cref="SecurityException">
		'''        if a security manager exists and its
		'''        {@code checkPermission} method doesn't allow
		'''        specifying a stream handler explicitly. </exception>
		''' <seealso cref=        java.lang.System#getProperty(java.lang.String) </seealso>
		''' <seealso cref=        java.net.URL#setURLStreamHandlerFactory(
		'''                  java.net.URLStreamHandlerFactory) </seealso>
		''' <seealso cref=        java.net.URLStreamHandler </seealso>
		''' <seealso cref=        java.net.URLStreamHandlerFactory#createURLStreamHandler(
		'''                  java.lang.String) </seealso>
		''' <seealso cref=        SecurityManager#checkPermission </seealso>
		''' <seealso cref=        java.net.NetPermission </seealso>
		Public Sub New(ByVal protocol As String, ByVal host As String, ByVal port As Integer, ByVal file As String, ByVal handler As URLStreamHandler)
			If handler IsNot Nothing Then
				Dim sm As SecurityManager = System.securityManager
				If sm IsNot Nothing Then checkSpecifyHandler(sm)
			End If

			protocol = protocol.ToLower()
			Me.protocol = protocol
			If host IsNot Nothing Then

				''' <summary>
				''' if host is a literal IPv6 address,
				''' we will make it conform to RFC 2732
				''' </summary>
				If host.IndexOf(":"c) >= 0 AndAlso (Not host.StartsWith("[")) Then host = "[" & host & "]"
				Me.host = host

				If port < -1 Then Throw New MalformedURLException("Invalid port number :" & port)
				Me.port = port
				authority = If(port = -1, host, host & ":" & port)
			End If

			Dim parts As New Parts(file)
			path = parts.path
			query = parts.query

			If query IsNot Nothing Then
				Me.file = path & "?" & query
			Else
				Me.file = path
			End If
			ref = parts.ref

			' Note: we don't do validation of the URL here. Too risky to change
			' right now, but worth considering for future reference. -br
			handler = getURLStreamHandler(protocol)
			If handler Is Nothing AndAlso handler Is Nothing Then Throw New MalformedURLException("unknown protocol: " & protocol)
			Me.handler = handler
		End Sub

		''' <summary>
		''' Creates a {@code URL} object from the {@code String}
		''' representation.
		''' <p>
		''' This constructor is equivalent to a call to the two-argument
		''' constructor with a {@code null} first argument.
		''' </summary>
		''' <param name="spec">   the {@code String} to parse as a URL. </param>
		''' <exception cref="MalformedURLException">  if no protocol is specified, or an
		'''               unknown protocol is found, or {@code spec} is {@code null}. </exception>
		''' <seealso cref=        java.net.URL#URL(java.net.URL, java.lang.String) </seealso>
		Public Sub New(ByVal spec As String)
			Me.New(Nothing, spec)
		End Sub

		''' <summary>
		''' Creates a URL by parsing the given spec within a specified context.
		''' 
		''' The new URL is created from the given context URL and the spec
		''' argument as described in
		''' RFC2396 &quot;Uniform Resource Identifiers : Generic * Syntax&quot; :
		''' <blockquote><pre>
		'''          &lt;scheme&gt;://&lt;authority&gt;&lt;path&gt;?&lt;query&gt;#&lt;fragment&gt;
		''' </pre></blockquote>
		''' The reference is parsed into the scheme, authority, path, query and
		''' fragment parts. If the path component is empty and the scheme,
		''' authority, and query components are undefined, then the new URL is a
		''' reference to the current document. Otherwise, the fragment and query
		''' parts present in the spec are used in the new URL.
		''' <p>
		''' If the scheme component is defined in the given spec and does not match
		''' the scheme of the context, then the new URL is created as an absolute
		''' URL based on the spec alone. Otherwise the scheme component is inherited
		''' from the context URL.
		''' <p>
		''' If the authority component is present in the spec then the spec is
		''' treated as absolute and the spec authority and path will replace the
		''' context authority and path. If the authority component is absent in the
		''' spec then the authority of the new URL will be inherited from the
		''' context.
		''' <p>
		''' If the spec's path component begins with a slash character
		''' &quot;/&quot; then the
		''' path is treated as absolute and the spec path replaces the context path.
		''' <p>
		''' Otherwise, the path is treated as a relative path and is appended to the
		''' context path, as described in RFC2396. Also, in this case,
		''' the path is canonicalized through the removal of directory
		''' changes made by occurrences of &quot;..&quot; and &quot;.&quot;.
		''' <p>
		''' For a more detailed description of URL parsing, refer to RFC2396.
		''' </summary>
		''' <param name="context">   the context in which to parse the specification. </param>
		''' <param name="spec">      the {@code String} to parse as a URL. </param>
		''' <exception cref="MalformedURLException">  if no protocol is specified, or an
		'''               unknown protocol is found, or {@code spec} is {@code null}. </exception>
		''' <seealso cref=        java.net.URL#URL(java.lang.String, java.lang.String,
		'''                  int, java.lang.String) </seealso>
		''' <seealso cref=        java.net.URLStreamHandler </seealso>
		''' <seealso cref=        java.net.URLStreamHandler#parseURL(java.net.URL,
		'''                  java.lang.String, int, int) </seealso>
		Public Sub New(ByVal context As URL, ByVal spec As String)
			Me.New(context, spec, Nothing)
		End Sub

		''' <summary>
		''' Creates a URL by parsing the given spec with the specified handler
		''' within a specified context. If the handler is null, the parsing
		''' occurs as with the two argument constructor.
		''' </summary>
		''' <param name="context">   the context in which to parse the specification. </param>
		''' <param name="spec">      the {@code String} to parse as a URL. </param>
		''' <param name="handler">   the stream handler for the URL. </param>
		''' <exception cref="MalformedURLException">  if no protocol is specified, or an
		'''               unknown protocol is found, or {@code spec} is {@code null}. </exception>
		''' <exception cref="SecurityException">
		'''        if a security manager exists and its
		'''        {@code checkPermission} method doesn't allow
		'''        specifying a stream handler. </exception>
		''' <seealso cref=        java.net.URL#URL(java.lang.String, java.lang.String,
		'''                  int, java.lang.String) </seealso>
		''' <seealso cref=        java.net.URLStreamHandler </seealso>
		''' <seealso cref=        java.net.URLStreamHandler#parseURL(java.net.URL,
		'''                  java.lang.String, int, int) </seealso>
		Public Sub New(ByVal context As URL, ByVal spec As String, ByVal handler As URLStreamHandler)
			Dim original As String = spec
			Dim i, limit, c As Integer
			Dim start As Integer = 0
			Dim newProtocol As String = Nothing
			Dim aRef As Boolean=False
			Dim isRelative As Boolean = False

			' Check for permission to specify a handler
			If handler IsNot Nothing Then
				Dim sm As SecurityManager = System.securityManager
				If sm IsNot Nothing Then checkSpecifyHandler(sm)
			End If

			Try
				limit = spec.length()
				Do While (limit > 0) AndAlso (spec.Chars(limit - 1) <= " "c)
					limit -= 1 'eliminate trailing whitespace
				Loop
				Do While (start < limit) AndAlso (spec.Chars(start) <= " "c)
					start += 1 ' eliminate leading whitespace
				Loop

				If spec.regionMatches(True, start, "url:", 0, 4) Then start += 4
				If start < spec.length() AndAlso spec.Chars(start) = "#"c Then aRef=True
				i = start
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				Do While (Not aRef) AndAlso (i < limit) AndAlso ((c = spec.Chars(i)) <> "/"c)
					If c = AscW(":"c) Then

						Dim s As String = spec.Substring(start, i - start).ToLower()
						If isValidProtocol(s) Then
							newProtocol = s
							start = i + 1
						End If
						Exit Do
					End If
					i += 1
				Loop

				' Only use our context if the protocols match.
				protocol = newProtocol
				If (context IsNot Nothing) AndAlso ((newProtocol Is Nothing) OrElse newProtocol.equalsIgnoreCase(context.protocol)) Then
					' inherit the protocol handler from the context
					' if not specified to the constructor
					If handler Is Nothing Then handler = context.handler

					' If the context is a hierarchical URL scheme and the spec
					' contains a matching scheme then maintain backwards
					' compatibility and treat it as if the spec didn't contain
					' the scheme; see 5.2.3 of RFC2396
					If context.path IsNot Nothing AndAlso context.path.StartsWith("/") Then newProtocol = Nothing

					If newProtocol Is Nothing Then
						protocol = context.protocol
						authority = context.authority
						userInfo = context.userInfo
						host = context.host
						port = context.port
						file = context.file
						path = context.path
						isRelative = True
					End If
				End If

				If protocol Is Nothing Then Throw New MalformedURLException("no protocol: " & original)

				' Get the protocol handler if not specified or the protocol
				' of the context could not be used
				handler = getURLStreamHandler(protocol)
				If handler Is Nothing AndAlso handler Is Nothing Then Throw New MalformedURLException("unknown protocol: " & protocol)

				Me.handler = handler

				i = spec.IndexOf("#"c, start)
				If i >= 0 Then
					ref = spec.Substring(i + 1, limit - (i + 1))
					limit = i
				End If

	'            
	'             * Handle special case inheritance of query and fragment
	'             * implied by RFC2396 section 5.2.2.
	'             
				If isRelative AndAlso start = limit Then
					query = context.query
					If ref Is Nothing Then ref = context.ref
				End If

				handler.parseURL(Me, spec, start, limit)

			Catch e As MalformedURLException
				Throw e
			Catch e As Exception
				Dim exception_Renamed As New MalformedURLException(e.message)
				exception_Renamed.initCause(e)
				Throw exception_Renamed
			End Try
		End Sub

	'    
	'     * Returns true if specified string is a valid protocol name.
	'     
		Private Function isValidProtocol(ByVal protocol As String) As Boolean
			Dim len As Integer = protocol.length()
			If len < 1 Then Return False
			Dim c As Char = protocol.Chars(0)
			If Not Char.IsLetter(c) Then Return False
			For i As Integer = 1 To len - 1
				c = protocol.Chars(i)
				If (Not Char.IsLetterOrDigit(c)) AndAlso c <> "."c AndAlso c <> "+"c AndAlso c <> "-"c Then Return False
			Next i
			Return True
		End Function

	'    
	'     * Checks for permission to specify a stream handler.
	'     
		Private Sub checkSpecifyHandler(ByVal sm As SecurityManager)
			sm.checkPermission(sun.security.util.SecurityConstants.SPECIFY_HANDLER_PERMISSION)
		End Sub

		''' <summary>
		''' Sets the fields of the URL. This is not a public method so that
		''' only URLStreamHandlers can modify URL fields. URLs are
		''' otherwise constant.
		''' </summary>
		''' <param name="protocol"> the name of the protocol to use </param>
		''' <param name="host"> the name of the host </param>
		'''   <param name="port"> the port number on the host </param>
		''' <param name="file"> the file on the host </param>
		''' <param name="ref"> the internal reference in the URL </param>
		Friend Sub [set](ByVal protocol As String, ByVal host As String, ByVal port As Integer, ByVal file As String, ByVal ref As String)
			SyncLock Me
				Me.protocol = protocol
				Me.host = host
				authority = If(port = -1, host, host & ":" & port)
				Me.port = port
				Me.file = file
				Me.ref = ref
	'             This is very important. We must recompute this after the
	'             * URL has been changed. 
				hashCode_Renamed = -1
				hostAddress = Nothing
				Dim q As Integer = file.LastIndexOf("?"c)
				If q <> -1 Then
					query = file.Substring(q+1)
					path = file.Substring(0, q)
				Else
					path = file
				End If
			End SyncLock
		End Sub

		''' <summary>
		''' Sets the specified 8 fields of the URL. This is not a public method so
		''' that only URLStreamHandlers can modify URL fields. URLs are otherwise
		''' constant.
		''' </summary>
		''' <param name="protocol"> the name of the protocol to use </param>
		''' <param name="host"> the name of the host </param>
		''' <param name="port"> the port number on the host </param>
		''' <param name="authority"> the authority part for the url </param>
		''' <param name="userInfo"> the username and password </param>
		''' <param name="path"> the file on the host </param>
		''' <param name="ref"> the internal reference in the URL </param>
		''' <param name="query"> the query part of this URL
		''' @since 1.3 </param>
		Friend Sub [set](ByVal protocol As String, ByVal host As String, ByVal port As Integer, ByVal authority As String, ByVal userInfo As String, ByVal path As String, ByVal query As String, ByVal ref As String)
			SyncLock Me
				Me.protocol = protocol
				Me.host = host
				Me.port = port
				Me.file = If(query Is Nothing, path, path & "?" & query)
				Me.userInfo = userInfo
				Me.path = path
				Me.ref = ref
	'             This is very important. We must recompute this after the
	'             * URL has been changed. 
				hashCode_Renamed = -1
				hostAddress = Nothing
				Me.query = query
				Me.authority = authority
			End SyncLock
		End Sub

		''' <summary>
		''' Gets the query part of this {@code URL}.
		''' </summary>
		''' <returns>  the query part of this {@code URL},
		''' or <CODE>null</CODE> if one does not exist
		''' @since 1.3 </returns>
		Public Property query As String
			Get
				Return query
			End Get
		End Property

		''' <summary>
		''' Gets the path part of this {@code URL}.
		''' </summary>
		''' <returns>  the path part of this {@code URL}, or an
		''' empty string if one does not exist
		''' @since 1.3 </returns>
		Public Property path As String
			Get
				Return path
			End Get
		End Property

		''' <summary>
		''' Gets the userInfo part of this {@code URL}.
		''' </summary>
		''' <returns>  the userInfo part of this {@code URL}, or
		''' <CODE>null</CODE> if one does not exist
		''' @since 1.3 </returns>
		Public Property userInfo As String
			Get
				Return userInfo
			End Get
		End Property

		''' <summary>
		''' Gets the authority part of this {@code URL}.
		''' </summary>
		''' <returns>  the authority part of this {@code URL}
		''' @since 1.3 </returns>
		Public Property authority As String
			Get
				Return authority
			End Get
		End Property

		''' <summary>
		''' Gets the port number of this {@code URL}.
		''' </summary>
		''' <returns>  the port number, or -1 if the port is not set </returns>
		Public Property port As Integer
			Get
				Return port
			End Get
		End Property

		''' <summary>
		''' Gets the default port number of the protocol associated
		''' with this {@code URL}. If the URL scheme or the URLStreamHandler
		''' for the URL do not define a default port number,
		''' then -1 is returned.
		''' </summary>
		''' <returns>  the port number
		''' @since 1.4 </returns>
		Public Property defaultPort As Integer
			Get
				Return handler.defaultPort
			End Get
		End Property

		''' <summary>
		''' Gets the protocol name of this {@code URL}.
		''' </summary>
		''' <returns>  the protocol of this {@code URL}. </returns>
		Public Property protocol As String
			Get
				Return protocol
			End Get
		End Property

		''' <summary>
		''' Gets the host name of this {@code URL}, if applicable.
		''' The format of the host conforms to RFC 2732, i.e. for a
		''' literal IPv6 address, this method will return the IPv6 address
		''' enclosed in square brackets ({@code '['} and {@code ']'}).
		''' </summary>
		''' <returns>  the host name of this {@code URL}. </returns>
		Public Property host As String
			Get
				Return host
			End Get
		End Property

		''' <summary>
		''' Gets the file name of this {@code URL}.
		''' The returned file portion will be
		''' the same as <CODE>getPath()</CODE>, plus the concatenation of
		''' the value of <CODE>getQuery()</CODE>, if any. If there is
		''' no query portion, this method and <CODE>getPath()</CODE> will
		''' return identical results.
		''' </summary>
		''' <returns>  the file name of this {@code URL},
		''' or an empty string if one does not exist </returns>
		Public Property file As String
			Get
				Return file
			End Get
		End Property

		''' <summary>
		''' Gets the anchor (also known as the "reference") of this
		''' {@code URL}.
		''' </summary>
		''' <returns>  the anchor (also known as the "reference") of this
		'''          {@code URL}, or <CODE>null</CODE> if one does not exist </returns>
		Public Property ref As String
			Get
				Return ref
			End Get
		End Property

		''' <summary>
		''' Compares this URL for equality with another object.<p>
		''' 
		''' If the given object is not a URL then this method immediately returns
		''' {@code false}.<p>
		''' 
		''' Two URL objects are equal if they have the same protocol, reference
		''' equivalent hosts, have the same port number on the host, and the same
		''' file and fragment of the file.<p>
		''' 
		''' Two hosts are considered equivalent if both host names can be resolved
		''' into the same IP addresses; else if either host name can't be
		''' resolved, the host names must be equal without regard to case; or both
		''' host names equal to null.<p>
		''' 
		''' Since hosts comparison requires name resolution, this operation is a
		''' blocking operation. <p>
		''' 
		''' Note: The defined behavior for {@code equals} is known to
		''' be inconsistent with virtual hosting in HTTP.
		''' </summary>
		''' <param name="obj">   the URL to compare against. </param>
		''' <returns>  {@code true} if the objects are the same;
		'''          {@code false} otherwise. </returns>
		Public Overrides Function Equals(ByVal obj As Object) As Boolean
			If Not(TypeOf obj Is URL) Then Return False
			Dim u2 As URL = CType(obj, URL)

			Return handler.Equals(Me, u2)
		End Function

		''' <summary>
		''' Creates an integer suitable for hash table indexing.<p>
		''' 
		''' The hash code is based upon all the URL components relevant for URL
		''' comparison. As such, this operation is a blocking operation.<p>
		''' </summary>
		''' <returns>  a hash code for this {@code URL}. </returns>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overrides Function GetHashCode() As Integer
			If hashCode_Renamed <> -1 Then Return hashCode_Renamed

			hashCode_Renamed = handler.hashCode(Me)
			Return hashCode_Renamed
		End Function

		''' <summary>
		''' Compares two URLs, excluding the fragment component.<p>
		''' 
		''' Returns {@code true} if this {@code URL} and the
		''' {@code other} argument are equal without taking the
		''' fragment component into consideration.
		''' </summary>
		''' <param name="other">   the {@code URL} to compare against. </param>
		''' <returns>  {@code true} if they reference the same remote object;
		'''          {@code false} otherwise. </returns>
		Public Function sameFile(ByVal other As URL) As Boolean
			Return handler.sameFile(Me, other)
		End Function

		''' <summary>
		''' Constructs a string representation of this {@code URL}. The
		''' string is created by calling the {@code toExternalForm}
		''' method of the stream protocol handler for this object.
		''' </summary>
		''' <returns>  a string representation of this object. </returns>
		''' <seealso cref=     java.net.URL#URL(java.lang.String, java.lang.String, int,
		'''                  java.lang.String) </seealso>
		''' <seealso cref=     java.net.URLStreamHandler#toExternalForm(java.net.URL) </seealso>
		Public Overrides Function ToString() As String
			Return toExternalForm()
		End Function

		''' <summary>
		''' Constructs a string representation of this {@code URL}. The
		''' string is created by calling the {@code toExternalForm}
		''' method of the stream protocol handler for this object.
		''' </summary>
		''' <returns>  a string representation of this object. </returns>
		''' <seealso cref=     java.net.URL#URL(java.lang.String, java.lang.String,
		'''                  int, java.lang.String) </seealso>
		''' <seealso cref=     java.net.URLStreamHandler#toExternalForm(java.net.URL) </seealso>
		Public Function toExternalForm() As String
			Return handler.toExternalForm(Me)
		End Function

		''' <summary>
		''' Returns a <seealso cref="java.net.URI"/> equivalent to this URL.
		''' This method functions in the same way as {@code new URI (this.toString())}.
		''' <p>Note, any URL instance that complies with RFC 2396 can be converted
		''' to a URI. However, some URLs that are not strictly in compliance
		''' can not be converted to a URI.
		''' </summary>
		''' <exception cref="URISyntaxException"> if this URL is not formatted strictly according to
		'''            to RFC2396 and cannot be converted to a URI.
		''' </exception>
		''' <returns>    a URI instance equivalent to this URL.
		''' @since 1.5 </returns>
		Public Function toURI() As URI
			Return New URI(ToString())
		End Function

		''' <summary>
		''' Returns a <seealso cref="java.net.URLConnection URLConnection"/> instance that
		''' represents a connection to the remote object referred to by the
		''' {@code URL}.
		''' 
		''' <P>A new instance of <seealso cref="java.net.URLConnection URLConnection"/> is
		''' created every time when invoking the
		''' {@link java.net.URLStreamHandler#openConnection(URL)
		''' URLStreamHandler.openConnection(URL)} method of the protocol handler for
		''' this URL.</P>
		''' 
		''' <P>It should be noted that a URLConnection instance does not establish
		''' the actual network connection on creation. This will happen only when
		''' calling <seealso cref="java.net.URLConnection#connect() URLConnection.connect()"/>.</P>
		''' 
		''' <P>If for the URL's protocol (such as HTTP or JAR), there
		''' exists a public, specialized URLConnection subclass belonging
		''' to one of the following packages or one of their subpackages:
		''' java.lang, java.io, java.util, java.net, the connection
		''' returned will be of that subclass. For example, for HTTP an
		''' HttpURLConnection will be returned, and for JAR a
		''' JarURLConnection will be returned.</P>
		''' </summary>
		''' <returns>     a <seealso cref="java.net.URLConnection URLConnection"/> linking
		'''             to the URL. </returns>
		''' <exception cref="IOException">  if an I/O exception occurs. </exception>
		''' <seealso cref=        java.net.URL#URL(java.lang.String, java.lang.String,
		'''             int, java.lang.String) </seealso>
		Public Function openConnection() As URLConnection
			Return handler.openConnection(Me)
		End Function

		''' <summary>
		''' Same as <seealso cref="#openConnection()"/>, except that the connection will be
		''' made through the specified proxy; Protocol handlers that do not
		''' support proxing will ignore the proxy parameter and make a
		''' normal connection.
		''' 
		''' Invoking this method preempts the system's default ProxySelector
		''' settings.
		''' </summary>
		''' <param name="proxy"> the Proxy through which this connection
		'''             will be made. If direct connection is desired,
		'''             Proxy.NO_PROXY should be specified. </param>
		''' <returns>     a {@code URLConnection} to the URL. </returns>
		''' <exception cref="IOException">  if an I/O exception occurs. </exception>
		''' <exception cref="SecurityException"> if a security manager is present
		'''             and the caller doesn't have permission to connect
		'''             to the proxy. </exception>
		''' <exception cref="IllegalArgumentException"> will be thrown if proxy is null,
		'''             or proxy has the wrong type </exception>
		''' <exception cref="UnsupportedOperationException"> if the subclass that
		'''             implements the protocol handler doesn't support
		'''             this method. </exception>
		''' <seealso cref=        java.net.URL#URL(java.lang.String, java.lang.String,
		'''             int, java.lang.String) </seealso>
		''' <seealso cref=        java.net.URLConnection </seealso>
		''' <seealso cref=        java.net.URLStreamHandler#openConnection(java.net.URL,
		'''             java.net.Proxy)
		''' @since      1.5 </seealso>
		Public Function openConnection(ByVal proxy_Renamed As Proxy) As URLConnection
			If proxy_Renamed Is Nothing Then Throw New IllegalArgumentException("proxy can not be null")

			' Create a copy of Proxy as a security measure
			Dim p As Proxy = If(proxy_Renamed Is Proxy.NO_PROXY, Proxy.NO_PROXY, sun.net.ApplicationProxy.create(proxy_Renamed))
			Dim sm As SecurityManager = System.securityManager
			If p.type() <> Proxy.Type.DIRECT AndAlso sm IsNot Nothing Then
				Dim epoint As InetSocketAddress = CType(p.address(), InetSocketAddress)
				If epoint.unresolved Then
					sm.checkConnect(epoint.hostName, epoint.port)
				Else
					sm.checkConnect(epoint.address.hostAddress, epoint.port)
				End If
			End If
			Return handler.openConnection(Me, p)
		End Function

		''' <summary>
		''' Opens a connection to this {@code URL} and returns an
		''' {@code InputStream} for reading from that connection. This
		''' method is a shorthand for:
		''' <blockquote><pre>
		'''     openConnection().getInputStream()
		''' </pre></blockquote>
		''' </summary>
		''' <returns>     an input stream for reading from the URL connection. </returns>
		''' <exception cref="IOException">  if an I/O exception occurs. </exception>
		''' <seealso cref=        java.net.URL#openConnection() </seealso>
		''' <seealso cref=        java.net.URLConnection#getInputStream() </seealso>
		Public Function openStream() As java.io.InputStream
			Return openConnection().inputStream
		End Function

		''' <summary>
		''' Gets the contents of this URL. This method is a shorthand for:
		''' <blockquote><pre>
		'''     openConnection().getContent()
		''' </pre></blockquote>
		''' </summary>
		''' <returns>     the contents of this URL. </returns>
		''' <exception cref="IOException">  if an I/O exception occurs. </exception>
		''' <seealso cref=        java.net.URLConnection#getContent() </seealso>
		Public Property content As Object
			Get
				Return openConnection().content
			End Get
		End Property

		''' <summary>
		''' Gets the contents of this URL. This method is a shorthand for:
		''' <blockquote><pre>
		'''     openConnection().getContent(Class[])
		''' </pre></blockquote>
		''' </summary>
		''' <param name="classes"> an array of Java types </param>
		''' <returns>     the content object of this URL that is the first match of
		'''               the types specified in the classes array.
		'''               null if none of the requested types are supported. </returns>
		''' <exception cref="IOException">  if an I/O exception occurs. </exception>
		''' <seealso cref=        java.net.URLConnection#getContent(Class[])
		''' @since 1.3 </seealso>
		Public Function getContent(ByVal classes As  [Class]()) As Object
			Return openConnection().getContent(classes)
		End Function

		''' <summary>
		''' The URLStreamHandler factory.
		''' </summary>
		Friend Shared factory As URLStreamHandlerFactory

		''' <summary>
		''' Sets an application's {@code URLStreamHandlerFactory}.
		''' This method can be called at most once in a given Java Virtual
		''' Machine.
		''' 
		''' <p> The {@code URLStreamHandlerFactory} instance is used to
		''' construct a stream protocol handler from a protocol name.
		''' 
		''' <p> If there is a security manager, this method first calls
		''' the security manager's {@code checkSetFactory} method
		''' to ensure the operation is allowed.
		''' This could result in a SecurityException.
		''' </summary>
		''' <param name="fac">   the desired factory. </param>
		''' <exception cref="Error">  if the application has already set a factory. </exception>
		''' <exception cref="SecurityException">  if a security manager exists and its
		'''             {@code checkSetFactory} method doesn't allow
		'''             the operation. </exception>
		''' <seealso cref=        java.net.URL#URL(java.lang.String, java.lang.String,
		'''             int, java.lang.String) </seealso>
		''' <seealso cref=        java.net.URLStreamHandlerFactory </seealso>
		''' <seealso cref=        SecurityManager#checkSetFactory </seealso>
		Public Shared Property uRLStreamHandlerFactory As URLStreamHandlerFactory
			Set(ByVal fac As URLStreamHandlerFactory)
				SyncLock streamHandlerLock
					If factory IsNot Nothing Then Throw New [Error]("factory already defined")
					Dim security As SecurityManager = System.securityManager
					If security IsNot Nothing Then security.checkSetFactory()
					handlers.Clear()
					factory = fac
				End SyncLock
			End Set
		End Property

		''' <summary>
		''' A table of protocol handlers.
		''' </summary>
		Friend Shared handlers As New Dictionary(Of String, URLStreamHandler)
		Private Shared streamHandlerLock As New Object

		''' <summary>
		''' Returns the Stream Handler. </summary>
		''' <param name="protocol"> the protocol to use </param>
		Friend Shared Function getURLStreamHandler(ByVal protocol As String) As URLStreamHandler

			Dim handler As URLStreamHandler = handlers(protocol)
			If handler Is Nothing Then

				Dim checkedWithFactory As Boolean = False

				' Use the factory (if any)
				If factory IsNot Nothing Then
					handler = factory.createURLStreamHandler(protocol)
					checkedWithFactory = True
				End If

				' Try java protocol handler
				If handler Is Nothing Then
					Dim packagePrefixList As String = Nothing

				packagePrefixList = java.security.AccessController.doPrivileged(New sun.security.action.GetPropertyAction(protocolPathProp,""))
					If packagePrefixList <> "" Then packagePrefixList &= "|"

					' REMIND: decide whether to allow the "null" class prefix
					' or not.
				packagePrefixList &= "sun.net.www.protocol"

					Dim packagePrefixIter As New java.util.StringTokenizer(packagePrefixList, "|")

					Do While handler Is Nothing AndAlso packagePrefixIter.hasMoreTokens()

						Dim packagePrefix As String = packagePrefixIter.nextToken().Trim()
						Try
							Dim clsName As String = packagePrefix & "." & protocol & ".Handler"
							Dim cls As  [Class] = Nothing
							Try
								cls = Type.GetType(clsName)
							Catch e As  ClassNotFoundException
								Dim cl As  ClassLoader = ClassLoader.systemClassLoader
								If cl IsNot Nothing Then cls = cl.loadClass(clsName)
							End Try
							If cls IsNot Nothing Then handler = CType(cls.newInstance(), URLStreamHandler)
						Catch e As Exception
							' any number of exceptions can get thrown here
						End Try
					Loop
				End If

				SyncLock streamHandlerLock

					Dim handler2 As URLStreamHandler = Nothing

					' Check again with hashtable just in case another
					' thread created a handler since we last checked
					handler2 = handlers(protocol)

					If handler2 IsNot Nothing Then Return handler2

					' Check with factory if another thread set a
					' factory since our last check
					If (Not checkedWithFactory) AndAlso factory IsNot Nothing Then handler2 = factory.createURLStreamHandler(protocol)

					If handler2 IsNot Nothing Then handler = handler2

					' Insert this handler into the hashtable
					If handler IsNot Nothing Then handlers(protocol) = handler

				End SyncLock
			End If

			Return handler

		End Function

		''' <summary>
		''' @serialField    protocol String
		''' 
		''' @serialField    host String
		''' 
		''' @serialField    port int
		''' 
		''' @serialField    authority String
		''' 
		''' @serialField    file String
		''' 
		''' @serialField    ref String
		''' 
		''' @serialField    hashCode int
		''' 
		''' </summary>
		Private Shared ReadOnly serialPersistentFields As java.io.ObjectStreamField() = { New java.io.ObjectStreamField("protocol", GetType(String)), New java.io.ObjectStreamField("host", GetType(String)), New java.io.ObjectStreamField("port", GetType(Integer)), New java.io.ObjectStreamField("authority", GetType(String)), New java.io.ObjectStreamField("file", GetType(String)), New java.io.ObjectStreamField("ref", GetType(String)), New java.io.ObjectStreamField("hashCode", GetType(Integer)) }

		''' <summary>
		''' WriteObject is called to save the state of the URL to an
		''' ObjectOutputStream. The handler is not saved since it is
		''' specific to this system.
		''' 
		''' @serialData the default write object value. When read back in,
		''' the reader must ensure that calling getURLStreamHandler with
		''' the protocol variable returns a valid URLStreamHandler and
		''' throw an IOException if it does not.
		''' </summary>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Private Sub writeObject(ByVal s As java.io.ObjectOutputStream)
			s.defaultWriteObject() ' write the fields
		End Sub

		''' <summary>
		''' readObject is called to restore the state of the URL from the
		''' stream.  It reads the components of the URL and finds the local
		''' stream handler.
		''' </summary>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Private Sub readObject(ByVal s As java.io.ObjectInputStream)
			Dim gf As java.io.ObjectInputStream.GetField = s.readFields()
			Dim protocol_Renamed As String = CStr(gf.get("protocol", Nothing))
			If getURLStreamHandler(protocol_Renamed) Is Nothing Then Throw New java.io.IOException("unknown protocol: " & protocol_Renamed)
			Dim host_Renamed As String = CStr(gf.get("host", Nothing))
			Dim port_Renamed As Integer = gf.get("port", -1)
			Dim authority_Renamed As String = CStr(gf.get("authority", Nothing))
			Dim file_Renamed As String = CStr(gf.get("file", Nothing))
			Dim ref_Renamed As String = CStr(gf.get("ref", Nothing))
			Dim hashCode As Integer = gf.get("hashCode", -1)
			If authority_Renamed Is Nothing AndAlso ((host_Renamed IsNot Nothing AndAlso host_Renamed.length() > 0) OrElse port_Renamed <> -1) Then
				If host_Renamed Is Nothing Then host_Renamed = ""
				authority_Renamed = If(port_Renamed = -1, host_Renamed, host_Renamed & ":" & port_Renamed)
			End If
			tempState = New UrlDeserializedState(protocol_Renamed, host_Renamed, port_Renamed, authority_Renamed, file_Renamed, ref_Renamed, hashCode)
		End Sub

		''' <summary>
		''' Replaces the de-serialized object with an URL object.
		''' </summary>
		''' <returns> a newly created object from the deserialzed state.
		''' </returns>
		''' <exception cref="ObjectStreamException"> if a new object replacing this
		''' object could not be created </exception>

	   Private Function readResolve() As Object

			Dim handler As URLStreamHandler = Nothing
			' already been checked in readObject
			handler = getURLStreamHandler(tempState.protocol)

			Dim replacementURL As URL = Nothing
			If isBuiltinStreamHandler(handler.GetType().name) Then
				replacementURL = fabricateNewURL()
			Else
				replacementURL = deserializedFieldslds(handler)
			End If
			Return replacementURL
	   End Function

		Private Function setDeserializedFields(ByVal handler As URLStreamHandler) As URL
			Dim replacementURL As URL
			Dim userInfo_Renamed As String = Nothing
			Dim protocol_Renamed As String = tempState.protocol
			Dim host_Renamed As String = tempState.host
			Dim port_Renamed As Integer = tempState.port
			Dim authority_Renamed As String = tempState.authority
			Dim file_Renamed As String = tempState.file
			Dim ref_Renamed As String = tempState.ref
			Dim hashCode As Integer = tempState.hashCode


			' Construct authority part
			If authority_Renamed Is Nothing AndAlso ((host_Renamed IsNot Nothing AndAlso host_Renamed.length() > 0) OrElse port_Renamed <> -1) Then
				If host_Renamed Is Nothing Then host_Renamed = ""
				authority_Renamed = If(port_Renamed = -1, host_Renamed, host_Renamed & ":" & port_Renamed)

				' Handle hosts with userInfo in them
				Dim at As Integer = host_Renamed.LastIndexOf("@"c)
				If at <> -1 Then
					userInfo_Renamed = host_Renamed.Substring(0, at)
					host_Renamed = host_Renamed.Substring(at+1)
				End If
			ElseIf authority_Renamed IsNot Nothing Then
				' Construct user info part
				Dim ind As Integer = authority_Renamed.IndexOf("@"c)
				If ind <> -1 Then userInfo_Renamed = authority_Renamed.Substring(0, ind)
			End If

			' Construct path and query part
			Dim path_Renamed As String = Nothing
			Dim query_Renamed As String = Nothing
			If file_Renamed IsNot Nothing Then
				' Fix: only do this if hierarchical?
				Dim q As Integer = file_Renamed.LastIndexOf("?"c)
				If q <> -1 Then
					query_Renamed = file_Renamed.Substring(q+1)
					path_Renamed = file_Renamed.Substring(0, q)
				Else
					path_Renamed = file_Renamed
				End If
			End If

			If port_Renamed = -1 Then port_Renamed = 0
			' Set the object fields.
			Me.protocol = protocol_Renamed
			Me.host = host_Renamed
			Me.port = port_Renamed
			Me.file = file_Renamed
			Me.authority = authority_Renamed
			Me.ref = ref_Renamed
			Me.hashCode_Renamed = hashCode
			Me.handler = handler
			Me.query = query_Renamed
			Me.path = path_Renamed
			Me.userInfo = userInfo_Renamed
			replacementURL = Me
			Return replacementURL
		End Function

		Private Function fabricateNewURL() As URL
			' create URL string from deserialized object
			Dim replacementURL As URL = Nothing
			Dim urlString As String = tempState.reconstituteUrlString()

			Try
				replacementURL = New URL(urlString)
			Catch mEx As MalformedURLException
				resetState()
				Dim invoEx As New java.io.InvalidObjectException("Malformed URL: " & urlString)
				invoEx.initCause(mEx)
				Throw invoEx
			End Try
			replacementURL.serializedHashCode = tempState.hashCode
			resetState()
			Return replacementURL
		End Function

		Private Function isBuiltinStreamHandler(ByVal handlerClassName As String) As Boolean
			Return (handlerClassName.StartsWith(BUILTIN_HANDLERS_PREFIX))
		End Function

		Private Sub resetState()
			Me.protocol = Nothing
			Me.host = Nothing
			Me.port = -1
			Me.file = Nothing
			Me.authority = Nothing
			Me.ref = Nothing
			Me.hashCode_Renamed = -1
			Me.handler = Nothing
			Me.query = Nothing
			Me.path = Nothing
			Me.userInfo = Nothing
			Me.tempState = Nothing
		End Sub

		Private Property serializedHashCode As Integer
			Set(ByVal hc As Integer)
				Me.hashCode_Renamed = hc
			End Set
		End Property
	End Class

	Friend Class Parts
		Friend path, query, ref As String

		Friend Sub New(ByVal file As String)
			Dim ind As Integer = file.IndexOf("#"c)
			ref = If(ind < 0, Nothing, file.Substring(ind + 1))
			file = If(ind < 0, file, file.Substring(0, ind))
			Dim q As Integer = file.LastIndexOf("?"c)
			If q <> -1 Then
				query = file.Substring(q+1)
				path = file.Substring(0, q)
			Else
				path = file
			End If
		End Sub

		Friend Overridable Property path As String
			Get
				Return path
			End Get
		End Property

		Friend Overridable Property query As String
			Get
				Return query
			End Get
		End Property

		Friend Overridable Property ref As String
			Get
				Return ref
			End Get
		End Property
	End Class

	Friend NotInheritable Class UrlDeserializedState
		Private ReadOnly protocol As String
		Private ReadOnly host As String
		Private ReadOnly port As Integer
		Private ReadOnly authority As String
		Private ReadOnly file As String
		Private ReadOnly ref As String
		Private ReadOnly hashCode As Integer

		Public Sub New(ByVal protocol As String, ByVal host As String, ByVal port As Integer, ByVal authority As String, ByVal file As String, ByVal ref As String, ByVal GetHashCode As Integer)
			Me.protocol = protocol
			Me.host = host
			Me.port = port
			Me.authority = authority
			Me.file = file
			Me.ref = ref
			Me.hashCode = hashCode
		End Sub

		Friend Property protocol As String
			Get
				Return protocol
			End Get
		End Property

		Friend Property host As String
			Get
				Return host
			End Get
		End Property

		Friend Property authority As String
			Get
				Return authority
			End Get
		End Property

		Friend Property port As Integer
			Get
				Return port
			End Get
		End Property

		Friend Property file As String
			Get
				Return file
			End Get
		End Property

		Friend Property ref As String
			Get
				Return ref
			End Get
		End Property

		Friend Property hashCode As Integer
			Get
				Return hashCode
			End Get
		End Property

		Friend Function reconstituteUrlString() As String

			' pre-compute length of StringBuilder
			Dim len As Integer = protocol.length() + 1
			If authority IsNot Nothing AndAlso authority.length() > 0 Then len += 2 + authority.length()
			If file IsNot Nothing Then len += file.length()
			If ref IsNot Nothing Then len += 1 + ref.length()
			Dim result As New StringBuilder(len)
			result.append(protocol)
			result.append(":")
			If authority IsNot Nothing AndAlso authority.length() > 0 Then
				result.append("//")
				result.append(authority)
			End If
			If file IsNot Nothing Then result.append(file)
			If ref IsNot Nothing Then
				result.append("#")
				result.append(ref)
			End If
			Return result.ToString()
		End Function
	End Class

End Namespace