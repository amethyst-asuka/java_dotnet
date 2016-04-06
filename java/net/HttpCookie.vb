Imports System
Imports System.Collections.Generic

'
' * Copyright (c) 2005, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' An HttpCookie object represents an HTTP cookie, which carries state
	''' information between server and user agent. Cookie is widely adopted
	''' to create stateful sessions.
	''' 
	''' <p> There are 3 HTTP cookie specifications:
	''' <blockquote>
	'''   Netscape draft<br>
	'''   RFC 2109 - <a href="http://www.ietf.org/rfc/rfc2109.txt">
	''' <i>http://www.ietf.org/rfc/rfc2109.txt</i></a><br>
	'''   RFC 2965 - <a href="http://www.ietf.org/rfc/rfc2965.txt">
	''' <i>http://www.ietf.org/rfc/rfc2965.txt</i></a>
	''' </blockquote>
	''' 
	''' <p> HttpCookie class can accept all these 3 forms of syntax.
	''' 
	''' @author Edward Wang
	''' @since 1.6
	''' </summary>
	Public NotInheritable Class HttpCookie
		Implements Cloneable

		' ---------------- Fields --------------

		' The value of the cookie itself.
		Private ReadOnly name As String ' NAME= ... "$Name" style is reserved
		Private value As String ' value of NAME

		' Attributes encoded in the header's cookie fields.
		Private comment As String ' Comment=VALUE ... describes cookie's use
		Private commentURL As String ' CommentURL="http URL" ... describes cookie's use
		Private toDiscard As Boolean ' Discard ... discard cookie unconditionally
		Private domain As String ' Domain=VALUE ... domain that sees cookie
		Private maxAge As Long = MAX_AGE_UNSPECIFIED ' Max-Age=VALUE ... cookies auto-expire
		Private path As String ' Path=VALUE ... URLs that see the cookie
		Private portlist As String ' Port[="portlist"] ... the port cookie may be returned to
		Private secure As Boolean ' Secure ... e.g. use SSL
		Private httpOnly As Boolean ' HttpOnly ... i.e. not accessible to scripts
		Private version As Integer = 1 ' Version=1 ... RFC 2965 style

		' The original header this cookie was consructed from, if it was
		' constructed by parsing a header, otherwise null.
		Private ReadOnly header_Renamed As String

		' Hold the creation time (in seconds) of the http cookie for later
		' expiration calculation
		Private ReadOnly whenCreated As Long

		' Since the positive and zero max-age have their meanings,
		' this value serves as a hint as 'not specify max-age'
		Private Const MAX_AGE_UNSPECIFIED As Long = -1

		' date formats used by Netscape's cookie draft
		' as well as formats seen on various sites
		Private Shared ReadOnly COOKIE_DATE_FORMATS As String() = { "EEE',' dd-MMM-yyyy HH:mm:ss 'GMT'", "EEE',' dd MMM yyyy HH:mm:ss 'GMT'", "EEE MMM dd yyyy HH:mm:ss 'GMT'Z", "EEE',' dd-MMM-yy HH:mm:ss 'GMT'", "EEE',' dd MMM yy HH:mm:ss 'GMT'", "EEE MMM dd yy HH:mm:ss 'GMT'Z" }

		' constant strings represent set-cookie header token
		Private Const SET_COOKIE As String = "set-cookie:"
		Private Const SET_COOKIE2 As String = "set-cookie2:"

		' ---------------- Ctors --------------

		''' <summary>
		''' Constructs a cookie with a specified name and value.
		''' 
		''' <p> The name must conform to RFC 2965. That means it can contain
		''' only ASCII alphanumeric characters and cannot contain commas,
		''' semicolons, or white space or begin with a $ character. The cookie's
		''' name cannot be changed after creation.
		''' 
		''' <p> The value can be anything the server chooses to send. Its
		''' value is probably of interest only to the server. The cookie's
		''' value can be changed after creation with the
		''' {@code setValue} method.
		''' 
		''' <p> By default, cookies are created according to the RFC 2965
		''' cookie specification. The version can be changed with the
		''' {@code setVersion} method.
		''' 
		''' </summary>
		''' <param name="name">
		'''         a {@code String} specifying the name of the cookie
		''' </param>
		''' <param name="value">
		'''         a {@code String} specifying the value of the cookie
		''' </param>
		''' <exception cref="IllegalArgumentException">
		'''          if the cookie name contains illegal characters </exception>
		''' <exception cref="NullPointerException">
		'''          if {@code name} is {@code null}
		''' </exception>
		''' <seealso cref= #setValue </seealso>
		''' <seealso cref= #setVersion </seealso>
		Public Sub New(  name As String,   value As String)
			Me.New(name, value, Nothing) 'header
		End Sub

		Private Sub New(  name As String,   value As String,   header As String)
			name = name.Trim()
			If name.length() = 0 OrElse (Not isToken(name)) OrElse name.Chars(0) = "$"c Then Throw New IllegalArgumentException("Illegal cookie name")

			Me.name = name
			Me.value = value
			toDiscard = False
			secure = False

			whenCreated = System.currentTimeMillis()
			portlist = Nothing
			Me.header_Renamed = header
		End Sub

		''' <summary>
		''' Constructs cookies from set-cookie or set-cookie2 header string.
		''' RFC 2965 section 3.2.2 set-cookie2 syntax indicates that one header line
		''' may contain more than one cookie definitions, so this is a static
		''' utility method instead of another constructor.
		''' </summary>
		''' <param name="header">
		'''         a {@code String} specifying the set-cookie header. The header
		'''         should start with "set-cookie", or "set-cookie2" token; or it
		'''         should have no leading token at all.
		''' </param>
		''' <returns>  a List of cookie parsed from header line string
		''' </returns>
		''' <exception cref="IllegalArgumentException">
		'''          if header string violates the cookie specification's syntax or
		'''          the cookie name contains illegal characters. </exception>
		''' <exception cref="NullPointerException">
		'''          if the header string is {@code null} </exception>
		Public Shared Function parse(  header As String) As IList(Of HttpCookie)
			Return parse(header, False)
		End Function

		' Private version of parse() that will store the original header used to
		' create the cookie, in the cookie itself. This can be useful for filtering
		' Set-Cookie[2] headers, using the internal parsing logic defined in this
		' class.
		Private Shared Function parse(  header As String,   retainHeader As Boolean) As IList(Of HttpCookie)

			Dim version_Renamed As Integer = guessCookieVersion(header)

			' if header start with set-cookie or set-cookie2, strip it off
			If startsWithIgnoreCase(header, SET_COOKIE2) Then
				header = header.Substring(SET_COOKIE2.length())
			ElseIf startsWithIgnoreCase(header, SET_COOKIE) Then
				header = header.Substring(SET_COOKIE.length())
			End If

			Dim cookies As IList(Of HttpCookie) = New List(Of HttpCookie)
			' The Netscape cookie may have a comma in its expires attribute, while
			' the comma is the delimiter in rfc 2965/2109 cookie header string.
			' so the parse logic is slightly different
			If version_Renamed = 0 Then
				' Netscape draft cookie
				Dim cookie As HttpCookie = parseInternal(header, retainHeader)
				cookie.version = 0
				cookies.Add(cookie)
			Else
				' rfc2965/2109 cookie
				' if header string contains more than one cookie,
				' it'll separate them with comma
				Dim cookieStrings As IList(Of String) = splitMultiCookies(header)
				For Each cookieStr As String In cookieStrings
					Dim cookie As HttpCookie = parseInternal(cookieStr, retainHeader)
					cookie.version = 1
					cookies.Add(cookie)
				Next cookieStr
			End If

			Return cookies
		End Function

		' ---------------- Public operations --------------

		''' <summary>
		''' Reports whether this HTTP cookie has expired or not.
		''' </summary>
		''' <returns>  {@code true} to indicate this HTTP cookie has expired;
		'''          otherwise, {@code false} </returns>
		Public Function hasExpired() As Boolean
			If maxAge = 0 Then Return True

			' if not specify max-age, this cookie should be
			' discarded when user agent is to be closed, but
			' it is not expired.
			If maxAge = MAX_AGE_UNSPECIFIED Then Return False

			Dim deltaSecond As Long = (System.currentTimeMillis() - whenCreated) / 1000
			If deltaSecond > maxAge Then
				Return True
			Else
				Return False
			End If
		End Function

		''' <summary>
		''' Specifies a comment that describes a cookie's purpose.
		''' The comment is useful if the browser presents the cookie
		''' to the user. Comments are not supported by Netscape Version 0 cookies.
		''' </summary>
		''' <param name="purpose">
		'''         a {@code String} specifying the comment to display to the user
		''' </param>
		''' <seealso cref=  #getComment </seealso>
		Public Property comment As String
			Set(  purpose As String)
				comment = purpose
			End Set
			Get
				Return comment
			End Get
		End Property


		''' <summary>
		''' Specifies a comment URL that describes a cookie's purpose.
		''' The comment URL is useful if the browser presents the cookie
		''' to the user. Comment URL is RFC 2965 only.
		''' </summary>
		''' <param name="purpose">
		'''         a {@code String} specifying the comment URL to display to the user
		''' </param>
		''' <seealso cref=  #getCommentURL </seealso>
		Public Property commentURL As String
			Set(  purpose As String)
				commentURL = purpose
			End Set
			Get
				Return commentURL
			End Get
		End Property


		''' <summary>
		''' Specify whether user agent should discard the cookie unconditionally.
		''' This is RFC 2965 only attribute.
		''' </summary>
		''' <param name="discard">
		'''         {@code true} indicates to discard cookie unconditionally
		''' </param>
		''' <seealso cref=  #getDiscard </seealso>
		Public Property discard As Boolean
			Set(  discard As Boolean)
				toDiscard = discard
			End Set
			Get
				Return toDiscard
			End Get
		End Property


		''' <summary>
		''' Specify the portlist of the cookie, which restricts the port(s)
		''' to which a cookie may be sent back in a Cookie header.
		''' </summary>
		''' <param name="ports">
		'''         a {@code String} specify the port list, which is comma separated
		'''         series of digits
		''' </param>
		''' <seealso cref=  #getPortlist </seealso>
		Public Property portlist As String
			Set(  ports As String)
				portlist = ports
			End Set
			Get
				Return portlist
			End Get
		End Property


		''' <summary>
		''' Specifies the domain within which this cookie should be presented.
		''' 
		''' <p> The form of the domain name is specified by RFC 2965. A domain
		''' name begins with a dot ({@code .foo.com}) and means that
		''' the cookie is visible to servers in a specified Domain Name System
		''' (DNS) zone (for example, {@code www.foo.com}, but not
		''' {@code a.b.foo.com}). By default, cookies are only returned
		''' to the server that sent them.
		''' </summary>
		''' <param name="pattern">
		'''         a {@code String} containing the domain name within which this
		'''         cookie is visible; form is according to RFC 2965
		''' </param>
		''' <seealso cref=  #getDomain </seealso>
		Public Property domain As String
			Set(  pattern As String)
				If pattern IsNot Nothing Then
					domain = pattern.ToLower()
				Else
					domain = pattern
				End If
			End Set
			Get
				Return domain
			End Get
		End Property


		''' <summary>
		''' Sets the maximum age of the cookie in seconds.
		''' 
		''' <p> A positive value indicates that the cookie will expire
		''' after that many seconds have passed. Note that the value is
		''' the <i>maximum</i> age when the cookie will expire, not the cookie's
		''' current age.
		''' 
		''' <p> A negative value means that the cookie is not stored persistently
		''' and will be deleted when the Web browser exits. A zero value causes the
		''' cookie to be deleted.
		''' </summary>
		''' <param name="expiry">
		'''         an integer specifying the maximum age of the cookie in seconds;
		'''         if zero, the cookie should be discarded immediately; otherwise,
		'''         the cookie's max age is unspecified.
		''' </param>
		''' <seealso cref=  #getMaxAge </seealso>
		Public Property maxAge As Long
			Set(  expiry As Long)
				maxAge = expiry
			End Set
			Get
				Return maxAge
			End Get
		End Property


		''' <summary>
		''' Specifies a path for the cookie to which the client should return
		''' the cookie.
		''' 
		''' <p> The cookie is visible to all the pages in the directory
		''' you specify, and all the pages in that directory's subdirectories.
		''' A cookie's path must include the servlet that set the cookie,
		''' for example, <i>/catalog</i>, which makes the cookie
		''' visible to all directories on the server under <i>/catalog</i>.
		''' 
		''' <p> Consult RFC 2965 (available on the Internet) for more
		''' information on setting path names for cookies.
		''' </summary>
		''' <param name="uri">
		'''         a {@code String} specifying a path
		''' </param>
		''' <seealso cref=  #getPath </seealso>
		Public Property path As String
			Set(  uri As String)
				path = uri
			End Set
			Get
				Return path
			End Get
		End Property


		''' <summary>
		''' Indicates whether the cookie should only be sent using a secure protocol,
		''' such as HTTPS or SSL.
		''' 
		''' <p> The default value is {@code false}.
		''' </summary>
		''' <param name="flag">
		'''         If {@code true}, the cookie can only be sent over a secure
		'''         protocol like HTTPS. If {@code false}, it can be sent over
		'''         any protocol.
		''' </param>
		''' <seealso cref=  #getSecure </seealso>
		Public Property secure As Boolean
			Set(  flag As Boolean)
				secure = flag
			End Set
			Get
				Return secure
			End Get
		End Property


		''' <summary>
		''' Returns the name of the cookie. The name cannot be changed after
		''' creation.
		''' </summary>
		''' <returns>  a {@code String} specifying the cookie's name </returns>
		Public Property name As String
			Get
				Return name
			End Get
		End Property

		''' <summary>
		''' Assigns a new value to a cookie after the cookie is created.
		''' If you use a binary value, you may want to use BASE64 encoding.
		''' 
		''' <p> With Version 0 cookies, values should not contain white space,
		''' brackets, parentheses, equals signs, commas, double quotes, slashes,
		''' question marks, at signs, colons, and semicolons. Empty values may not
		''' behave the same way on all browsers.
		''' </summary>
		''' <param name="newValue">
		'''         a {@code String} specifying the new value
		''' </param>
		''' <seealso cref=  #getValue </seealso>
		Public Property value As String
			Set(  newValue As String)
				value = newValue
			End Set
			Get
				Return value
			End Get
		End Property


		''' <summary>
		''' Returns the version of the protocol this cookie complies with. Version 1
		''' complies with RFC 2965/2109, and version 0 complies with the original
		''' cookie specification drafted by Netscape. Cookies provided by a browser
		''' use and identify the browser's cookie version.
		''' </summary>
		''' <returns>  0 if the cookie complies with the original Netscape
		'''          specification; 1 if the cookie complies with RFC 2965/2109
		''' </returns>
		''' <seealso cref=  #setVersion </seealso>
		Public Property version As Integer
			Get
				Return version
			End Get
			Set(  v As Integer)
				If v <> 0 AndAlso v <> 1 Then Throw New IllegalArgumentException("cookie version should be 0 or 1")
    
				version = v
			End Set
		End Property


		''' <summary>
		''' Returns {@code true} if this cookie contains the <i>HttpOnly</i>
		''' attribute. This means that the cookie should not be accessible to
		''' scripting engines, like javascript.
		''' </summary>
		''' <returns>  {@code true} if this cookie should be considered HTTPOnly
		''' </returns>
		''' <seealso cref=  #setHttpOnly(boolean) </seealso>
		Public Property httpOnly As Boolean
			Get
				Return httpOnly
			End Get
			Set(  httpOnly As Boolean)
				Me.httpOnly = httpOnly
			End Set
		End Property


		''' <summary>
		''' The utility method to check whether a host name is in a domain or not.
		''' 
		''' <p> This concept is described in the cookie specification.
		''' To understand the concept, some terminologies need to be defined first:
		''' <blockquote>
		''' effective host name = hostname if host name contains dot<br>
		''' &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
		''' &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;or = hostname.local if not
		''' </blockquote>
		''' <p>Host A's name domain-matches host B's if:
		''' <blockquote><ul>
		'''   <li>their host name strings string-compare equal; or</li>
		'''   <li>A is a HDN string and has the form NB, where N is a non-empty
		'''   name string, B has the form .B', and B' is a HDN string.  (So,
		'''   x.y.com domain-matches .Y.com but not Y.com.)</li>
		''' </ul></blockquote>
		''' 
		''' <p>A host isn't in a domain (RFC 2965 sec. 3.3.2) if:
		''' <blockquote><ul>
		'''   <li>The value for the Domain attribute contains no embedded dots,
		'''   and the value is not .local.</li>
		'''   <li>The effective host name that derives from the request-host does
		'''   not domain-match the Domain attribute.</li>
		'''   <li>The request-host is a HDN (not IP address) and has the form HD,
		'''   where D is the value of the Domain attribute, and H is a string
		'''   that contains one or more dots.</li>
		''' </ul></blockquote>
		''' 
		''' <p>Examples:
		''' <blockquote><ul>
		'''   <li>A Set-Cookie2 from request-host y.x.foo.com for Domain=.foo.com
		'''   would be rejected, because H is y.x and contains a dot.</li>
		'''   <li>A Set-Cookie2 from request-host x.foo.com for Domain=.foo.com
		'''   would be accepted.</li>
		'''   <li>A Set-Cookie2 with Domain=.com or Domain=.com., will always be
		'''   rejected, because there is no embedded dot.</li>
		'''   <li>A Set-Cookie2 from request-host example for Domain=.local will
		'''   be accepted, because the effective host name for the request-
		'''   host is example.local, and example.local domain-matches .local.</li>
		''' </ul></blockquote>
		''' </summary>
		''' <param name="domain">
		'''         the domain name to check host name with
		''' </param>
		''' <param name="host">
		'''         the host name in question
		''' </param>
		''' <returns>  {@code true} if they domain-matches; {@code false} if not </returns>
		Public Shared Function domainMatches(  domain As String,   host As String) As Boolean
			If domain Is Nothing OrElse host Is Nothing Then Return False

			' if there's no embedded dot in domain and domain is not .local
			Dim isLocalDomain As Boolean = ".local".equalsIgnoreCase(domain)
			Dim embeddedDotInDomain As Integer = domain.IndexOf("."c)
			If embeddedDotInDomain = 0 Then embeddedDotInDomain = domain.IndexOf("."c, 1)
			If (Not isLocalDomain) AndAlso (embeddedDotInDomain = -1 OrElse embeddedDotInDomain = domain.length() - 1) Then Return False

			' if the host name contains no dot and the domain name
			' is .local or host.local
			Dim firstDotInHost As Integer = host.IndexOf("."c)
			If firstDotInHost = -1 AndAlso (isLocalDomain OrElse domain.equalsIgnoreCase(host & ".local")) Then Return True

			Dim domainLength As Integer = domain.length()
			Dim lengthDiff As Integer = host.length() - domainLength
			If lengthDiff = 0 Then
				' if the host name and the domain name are just string-compare euqal
				Return host.equalsIgnoreCase(domain)
			ElseIf lengthDiff > 0 Then
				' need to check H & D component
				Dim H As String = host.Substring(0, lengthDiff)
				Dim D As String = host.Substring(lengthDiff)

				Return (H.IndexOf("."c) = -1 AndAlso D.equalsIgnoreCase(domain))
			ElseIf lengthDiff = -1 Then
				' if domain is actually .host
				Return (domain.Chars(0) = "."c AndAlso host.equalsIgnoreCase(domain.Substring(1)))
			End If

			Return False
		End Function

		''' <summary>
		''' Constructs a cookie header string representation of this cookie,
		''' which is in the format defined by corresponding cookie specification,
		''' but without the leading "Cookie:" token.
		''' </summary>
		''' <returns>  a string form of the cookie. The string has the defined format </returns>
		Public Overrides Function ToString() As String
			If version > 0 Then
				Return toRFC2965HeaderString()
			Else
				Return toNetscapeHeaderString()
			End If
		End Function

		''' <summary>
		''' Test the equality of two HTTP cookies.
		''' 
		''' <p> The result is {@code true} only if two cookies come from same domain
		''' (case-insensitive), have same name (case-insensitive), and have same path
		''' (case-sensitive).
		''' </summary>
		''' <returns>  {@code true} if two HTTP cookies equal to each other;
		'''          otherwise, {@code false} </returns>
		Public Overrides Function Equals(  obj As Object) As Boolean
			If obj Is Me Then Return True
			If Not(TypeOf obj Is HttpCookie) Then Return False
			Dim other As HttpCookie = CType(obj, HttpCookie)

			' One http cookie equals to another cookie (RFC 2965 sec. 3.3.3) if:
			'   1. they come from same domain (case-insensitive),
			'   2. have same name (case-insensitive),
			'   3. and have same path (case-sensitive).
			Return equalsIgnoreCase(name, other.name) AndAlso equalsIgnoreCase(domain, other.domain) AndAlso java.util.Objects.Equals(path, other.path)
		End Function

		''' <summary>
		''' Returns the hash code of this HTTP cookie. The result is the sum of
		''' hash code value of three significant components of this cookie: name,
		''' domain, and path. That is, the hash code is the value of the expression:
		''' <blockquote>
		''' getName().toLowerCase().hashCode()<br>
		''' + getDomain().toLowerCase().hashCode()<br>
		''' + getPath().hashCode()
		''' </blockquote>
		''' </summary>
		''' <returns>  this HTTP cookie's hash code </returns>
		Public Overrides Function GetHashCode() As Integer
			Dim h1 As Integer = name.ToLower().GetHashCode()
			Dim h2 As Integer = If(domain IsNot Nothing, domain.ToLower().GetHashCode(), 0)
			Dim h3 As Integer = If(path IsNot Nothing, path.GetHashCode(), 0)

			Return h1 + h2 + h3
		End Function

		''' <summary>
		''' Create and return a copy of this object.
		''' </summary>
		''' <returns>  a clone of this HTTP cookie </returns>
		Public Overrides Function clone() As Object
			Try
				Return MyBase.clone()
			Catch e As CloneNotSupportedException
				Throw New RuntimeException(e.Message)
			End Try
		End Function

		' ---------------- Private operations --------------

		' Note -- disabled for now to allow full Netscape compatibility
		' from RFC 2068, token special case characters
		'
		' private static final String tspecials = "()<>@,;:\\\"/[]?={} \t";
		Private Const tspecials As String = ",; " ' deliberately includes space

	'    
	'     * Tests a string and returns true if the string counts as a token.
	'     *
	'     * @param  value
	'     *         the {@code String} to be tested
	'     *
	'     * @return  {@code true} if the {@code String} is a token;
	'     *          {@code false} if it is not
	'     
		Private Shared Function isToken(  value As String) As Boolean
			Dim len As Integer = value.length()

			For i As Integer = 0 To len - 1
				Dim c As Char = value.Chars(i)

				If AscW(c) < &H20 OrElse c >= &H7f OrElse tspecials.IndexOf(c) <> -1 Then Return False
			Next i
			Return True
		End Function

	'    
	'     * Parse header string to cookie object.
	'     *
	'     * @param  header
	'     *         header string; should contain only one NAME=VALUE pair
	'     *
	'     * @return  an HttpCookie being extracted
	'     *
	'     * @throws  IllegalArgumentException
	'     *          if header string violates the cookie specification
	'     
		Private Shared Function parseInternal(  header As String,   retainHeader As Boolean) As HttpCookie
			Dim cookie As HttpCookie = Nothing
			Dim namevaluePair As String = Nothing

			Dim tokenizer As New java.util.StringTokenizer(header, ";")

			' there should always have at least on name-value pair;
			' it's cookie's name
			Try
				namevaluePair = tokenizer.nextToken()
				Dim index As Integer = namevaluePair.IndexOf("="c)
				If index <> -1 Then
					Dim name_Renamed As String = namevaluePair.Substring(0, index).Trim()
					Dim value_Renamed As String = namevaluePair.Substring(index + 1).Trim()
					If retainHeader Then
						cookie = New HttpCookie(name_Renamed, stripOffSurroundingQuote(value_Renamed), header)
					Else
						cookie = New HttpCookie(name_Renamed, stripOffSurroundingQuote(value_Renamed))
					End If
				Else
					' no "=" in name-value pair; it's an error
					Throw New IllegalArgumentException("Invalid cookie name-value pair")
				End If
			Catch ignored As java.util.NoSuchElementException
				Throw New IllegalArgumentException("Empty cookie header string")
			End Try

			' remaining name-value pairs are cookie's attributes
			Do While tokenizer.hasMoreTokens()
				namevaluePair = tokenizer.nextToken()
				Dim index As Integer = namevaluePair.IndexOf("="c)
				Dim name_Renamed, value_Renamed As String
				If index <> -1 Then
					name_Renamed = namevaluePair.Substring(0, index).Trim()
					value_Renamed = namevaluePair.Substring(index + 1).Trim()
				Else
					name_Renamed = namevaluePair.Trim()
					value_Renamed = Nothing
				End If

				' assign attribute to cookie
				assignAttribute(cookie, name_Renamed, value_Renamed)
			Loop

			Return cookie
		End Function

	'    
	'     * assign cookie attribute value to attribute name;
	'     * use a map to simulate method dispatch
	'     
		Friend Interface CookieAttributeAssignor
				Sub assign(  cookie As HttpCookie,   attrName As String,   attrValue As String)
		End Interface
		Friend Shared ReadOnly assignors As IDictionary(Of String, CookieAttributeAssignor) = New Dictionary(Of String, CookieAttributeAssignor)
		Shared Sub New()
			assignors.put("comment", New CookieAttributeAssignorAnonymousInnerClassHelper
			assignors.put("commenturl", New CookieAttributeAssignorAnonymousInnerClassHelper2
			assignors.put("discard", New CookieAttributeAssignorAnonymousInnerClassHelper3
			assignors.put("domain", New CookieAttributeAssignorAnonymousInnerClassHelper4
			assignors.put("max-age", New CookieAttributeAssignorAnonymousInnerClassHelper5
			assignors.put("path", New CookieAttributeAssignorAnonymousInnerClassHelper6
			assignors.put("port", New CookieAttributeAssignorAnonymousInnerClassHelper7
			assignors.put("secure", New CookieAttributeAssignorAnonymousInnerClassHelper8
			assignors.put("httponly", New CookieAttributeAssignorAnonymousInnerClassHelper9
			assignors.put("version", New CookieAttributeAssignorAnonymousInnerClassHelper10
			assignors.put("expires", New CookieAttributeAssignorAnonymousInnerClassHelper11 ' Netscape only
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'			sun.misc.SharedSecrets.setJavaNetHttpCookieAccess(New sun.misc.JavaNetHttpCookieAccess()
	'		{
	'				public List<HttpCookie> parse(String header)
	'				{
	'					Return HttpCookie.parse(header, True);
	'				}
	'
	'				public String header(HttpCookie cookie)
	'				{
	'					Return cookie.header;
	'				}
	'			}
		   )
		End Sub

		Private Class CookieAttributeAssignorAnonymousInnerClassHelper
			Implements CookieAttributeAssignor

			Public Overridable Sub assign(  cookie As HttpCookie,   attrName As String,   attrValue As String)
				If cookie.comment Is Nothing Then cookie.comment = attrValue
			End Sub
		End Class

		Private Class CookieAttributeAssignorAnonymousInnerClassHelper2
			Implements CookieAttributeAssignor

			Public Overridable Sub assign(  cookie As HttpCookie,   attrName As String,   attrValue As String)
				If cookie.commentURL Is Nothing Then cookie.commentURL = attrValue
			End Sub
		End Class

		Private Class CookieAttributeAssignorAnonymousInnerClassHelper3
			Implements CookieAttributeAssignor

			Public Overridable Sub assign(  cookie As HttpCookie,   attrName As String,   attrValue As String)
				cookie.discard = True
			End Sub
		End Class

		Private Class CookieAttributeAssignorAnonymousInnerClassHelper4
			Implements CookieAttributeAssignor

			Public Overridable Sub assign(  cookie As HttpCookie,   attrName As String,   attrValue As String)
				If cookie.domain Is Nothing Then cookie.domain = attrValue
			End Sub
		End Class

		Private Class CookieAttributeAssignorAnonymousInnerClassHelper5
			Implements CookieAttributeAssignor

			Public Overridable Sub assign(  cookie As HttpCookie,   attrName As String,   attrValue As String)
				Try
					Dim maxage As Long = Convert.ToInt64(attrValue)
					If cookie.maxAge = MAX_AGE_UNSPECIFIED Then cookie.maxAge = maxage
				Catch ignored As NumberFormatException
					Throw New IllegalArgumentException("Illegal cookie max-age attribute")
				End Try
			End Sub
		End Class

		Private Class CookieAttributeAssignorAnonymousInnerClassHelper6
			Implements CookieAttributeAssignor

			Public Overridable Sub assign(  cookie As HttpCookie,   attrName As String,   attrValue As String)
				If cookie.path Is Nothing Then cookie.path = attrValue
			End Sub
		End Class

		Private Class CookieAttributeAssignorAnonymousInnerClassHelper7
			Implements CookieAttributeAssignor

			Public Overridable Sub assign(  cookie As HttpCookie,   attrName As String,   attrValue As String)
				If cookie.portlist Is Nothing Then cookie.portlist = If(attrValue Is Nothing, "", attrValue)
			End Sub
		End Class

		Private Class CookieAttributeAssignorAnonymousInnerClassHelper8
			Implements CookieAttributeAssignor

			Public Overridable Sub assign(  cookie As HttpCookie,   attrName As String,   attrValue As String)
				cookie.secure = True
			End Sub
		End Class

		Private Class CookieAttributeAssignorAnonymousInnerClassHelper9
			Implements CookieAttributeAssignor

			Public Overridable Sub assign(  cookie As HttpCookie,   attrName As String,   attrValue As String)
				cookie.httpOnly = True
			End Sub
		End Class

		Private Class CookieAttributeAssignorAnonymousInnerClassHelper10
			Implements CookieAttributeAssignor

			Public Overridable Sub assign(  cookie As HttpCookie,   attrName As String,   attrValue As String)
				Try
					Dim version As Integer = Convert.ToInt32(attrValue)
					cookie.version = version
				Catch ignored As NumberFormatException
					' Just ignore bogus version, it will default to 0 or 1
				End Try
			End Sub
		End Class

		Private Class CookieAttributeAssignorAnonymousInnerClassHelper11
			Implements CookieAttributeAssignor

			Public Overridable Sub assign(  cookie As HttpCookie,   attrName As String,   attrValue As String)
				If cookie.maxAge = MAX_AGE_UNSPECIFIED Then cookie.maxAge = cookie.expiryDate2DeltaSeconds(attrValue)
			End Sub
		End Class
		Private Shared Sub assignAttribute(  cookie As HttpCookie,   attrName As String,   attrValue As String)
			' strip off the surrounding "-sign if there's any
			attrValue = stripOffSurroundingQuote(attrValue)

			Dim assignor As CookieAttributeAssignor = assignors(attrName.ToLower())
			If assignor IsNot Nothing Then
				assignor.assign(cookie, attrName, attrValue)
			Else
				' Ignore the attribute as per RFC 2965
			End If
		End Sub


	'    
	'     * Returns the original header this cookie was consructed from, if it was
	'     * constructed by parsing a header, otherwise null.
	'     
		Private Function header() As String
			Return header_Renamed
		End Function

	'    
	'     * Constructs a string representation of this cookie. The string format is
	'     * as Netscape spec, but without leading "Cookie:" token.
	'     
		Private Function toNetscapeHeaderString() As String
			Return name & "=" & value
		End Function

	'    
	'     * Constructs a string representation of this cookie. The string format is
	'     * as RFC 2965/2109, but without leading "Cookie:" token.
	'     
		Private Function toRFC2965HeaderString() As String
			Dim sb As New StringBuilder

			sb.append(name).append("=""").append(value).append(""""c)
			If path IsNot Nothing Then sb.append(";$Path=""").append(path).append(""""c)
			If domain IsNot Nothing Then sb.append(";$Domain=""").append(domain).append(""""c)
			If portlist IsNot Nothing Then sb.append(";$Port=""").append(portlist).append(""""c)

			Return sb.ToString()
		End Function

		Friend Shared ReadOnly GMT As java.util.TimeZone = java.util.TimeZone.getTimeZone("GMT")

	'    
	'     * @param  dateString
	'     *         a date string in one of the formats defined in Netscape cookie spec
	'     *
	'     * @return  delta seconds between this cookie's creation time and the time
	'     *          specified by dateString
	'     
		Private Function expiryDate2DeltaSeconds(  dateString As String) As Long
			Dim cal As DateTime? = New java.util.GregorianCalendar(GMT)
			For i As Integer = 0 To COOKIE_DATE_FORMATS.Length - 1
				Dim df As New java.text.SimpleDateFormat(COOKIE_DATE_FORMATS(i), java.util.Locale.US)
				cal.Value.set(1970, 0, 1, 0, 0, 0)
				df.timeZone = GMT
				df.lenient = False
				df.set2DigitYearStart(cal.Value.time)
				Try
					cal.Value.time = df.parse(dateString)
					If Not COOKIE_DATE_FORMATS(i).contains("yyyy") Then
						' 2-digit years following the standard set
						' out it rfc 6265
						Dim year As Integer = cal.Value.get(DateTime.YEAR)
						year = year Mod 100
						If year < 70 Then
							year += 2000
						Else
							year += 1900
						End If
						cal.Value.set(DateTime.YEAR, year)
					End If
					Return (cal.Value.timeInMillis - whenCreated) / 1000
				Catch e As Exception
					' Ignore, try the next date format
				End Try
			Next i
			Return 0
		End Function

	'    
	'     * try to guess the cookie version through set-cookie header string
	'     
		Private Shared Function guessCookieVersion(  header As String) As Integer
			Dim version_Renamed As Integer = 0

			header = header.ToLower()
			If header.IndexOf("expires=") <> -1 Then
				' only netscape cookie using 'expires'
				version_Renamed = 0
			ElseIf header.IndexOf("version=") <> -1 Then
				' version is mandatory for rfc 2965/2109 cookie
				version_Renamed = 1
			ElseIf header.IndexOf("max-age") <> -1 Then
				' rfc 2965/2109 use 'max-age'
				version_Renamed = 1
			ElseIf startsWithIgnoreCase(header, SET_COOKIE2) Then
				' only rfc 2965 cookie starts with 'set-cookie2'
				version_Renamed = 1
			End If

			Return version_Renamed
		End Function

		Private Shared Function stripOffSurroundingQuote(  str As String) As String
			If str IsNot Nothing AndAlso str.length() > 2 AndAlso str.Chars(0) = """"c AndAlso str.Chars(str.length() - 1) = """"c Then Return str.Substring(1, str.length() - 1 - 1)
			If str IsNot Nothing AndAlso str.length() > 2 AndAlso str.Chars(0) = "'"c AndAlso str.Chars(str.length() - 1) = "'"c Then Return str.Substring(1, str.length() - 1 - 1)
			Return str
		End Function

		Private Shared Function equalsIgnoreCase(  s As String,   t As String) As Boolean
			If s = t Then Return True
			If (s IsNot Nothing) AndAlso (t IsNot Nothing) Then Return s.equalsIgnoreCase(t)
			Return False
		End Function

		Private Shared Function startsWithIgnoreCase(  s As String,   start As String) As Boolean
			If s Is Nothing OrElse start Is Nothing Then Return False

			If s.length() >= start.length() AndAlso start.equalsIgnoreCase(s.Substring(0, start.length())) Then Return True

			Return False
		End Function

	'    
	'     * Split cookie header string according to rfc 2965:
	'     *   1) split where it is a comma;
	'     *   2) but not the comma surrounding by double-quotes, which is the comma
	'     *      inside port list or embeded URIs.
	'     *
	'     * @param  header
	'     *         the cookie header string to split
	'     *
	'     * @return  list of strings; never null
	'     
		Private Shared Function splitMultiCookies(  header As String) As IList(Of String)
			Dim cookies As IList(Of String) = New List(Of String)
			Dim quoteCount As Integer = 0
			Dim p, q As Integer

			p = 0
			q = 0
			Do While p < header.length()
				Dim c As Char = header.Chars(p)
				If c = """"c Then quoteCount += 1
				If c = ","c AndAlso (quoteCount Mod 2 = 0) Then
					' it is comma and not surrounding by double-quotes
					cookies.Add(header.Substring(q, p - q))
					q = p + 1
				End If
				p += 1
			Loop

			cookies.Add(header.Substring(q))

			Return cookies
		End Function
	End Class

End Namespace