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
	''' CookieManager provides a concrete implementation of <seealso cref="CookieHandler"/>,
	''' which separates the storage of cookies from the policy surrounding accepting
	''' and rejecting cookies. A CookieManager is initialized with a <seealso cref="CookieStore"/>
	''' which manages storage, and a <seealso cref="CookiePolicy"/> object, which makes
	''' policy decisions on cookie acceptance/rejection.
	''' 
	''' <p> The HTTP cookie management in java.net package looks like:
	''' <blockquote>
	''' <pre>{@code
	'''                  use
	''' CookieHandler <------- HttpURLConnection
	'''       ^
	'''       | impl
	'''       |         use
	''' CookieManager -------> CookiePolicy
	'''             |   use
	'''             |--------> HttpCookie
	'''             |              ^
	'''             |              | use
	'''             |   use        |
	'''             |--------> CookieStore
	'''                            ^
	'''                            | impl
	'''                            |
	'''                  Internal in-memory implementation
	''' }</pre>
	''' <ul>
	'''   <li>
	'''     CookieHandler is at the core of cookie management. User can call
	'''     CookieHandler.setDefault to set a concrete CookieHanlder implementation
	'''     to be used.
	'''   </li>
	'''   <li>
	'''     CookiePolicy.shouldAccept will be called by CookieManager.put to see whether
	'''     or not one cookie should be accepted and put into cookie store. User can use
	'''     any of three pre-defined CookiePolicy, namely ACCEPT_ALL, ACCEPT_NONE and
	'''     ACCEPT_ORIGINAL_SERVER, or user can define his own CookiePolicy implementation
	'''     and tell CookieManager to use it.
	'''   </li>
	'''   <li>
	'''     CookieStore is the place where any accepted HTTP cookie is stored in.
	'''     If not specified when created, a CookieManager instance will use an internal
	'''     in-memory implementation. Or user can implements one and tell CookieManager
	'''     to use it.
	'''   </li>
	'''   <li>
	'''     Currently, only CookieStore.add(URI, HttpCookie) and CookieStore.get(URI)
	'''     are used by CookieManager. Others are for completeness and might be needed
	'''     by a more sophisticated CookieStore implementation, e.g. a NetscapeCookieSotre.
	'''   </li>
	''' </ul>
	''' </blockquote>
	''' 
	''' <p>There're various ways user can hook up his own HTTP cookie management behavior, e.g.
	''' <blockquote>
	''' <ul>
	'''   <li>Use CookieHandler.setDefault to set a brand new <seealso cref="CookieHandler"/> implementation
	'''   <li>Let CookieManager be the default <seealso cref="CookieHandler"/> implementation,
	'''       but implement user's own <seealso cref="CookieStore"/> and <seealso cref="CookiePolicy"/>
	'''       and tell default CookieManager to use them:
	'''     <blockquote><pre>
	'''       // this should be done at the beginning of an HTTP session
	'''       CookieHandler.setDefault(new CookieManager(new MyCookieStore(), new MyCookiePolicy()));
	'''     </pre></blockquote>
	'''   <li>Let CookieManager be the default <seealso cref="CookieHandler"/> implementation, but
	'''       use customized <seealso cref="CookiePolicy"/>:
	'''     <blockquote><pre>
	'''       // this should be done at the beginning of an HTTP session
	'''       CookieHandler.setDefault(new CookieManager());
	'''       // this can be done at any point of an HTTP session
	'''       ((CookieManager)CookieHandler.getDefault()).setCookiePolicy(new MyCookiePolicy());
	'''     </pre></blockquote>
	''' </ul>
	''' </blockquote>
	''' 
	''' <p>The implementation conforms to <a href="http://www.ietf.org/rfc/rfc2965.txt">RFC 2965</a>, section 3.3.
	''' </summary>
	''' <seealso cref= CookiePolicy
	''' @author Edward Wang
	''' @since 1.6 </seealso>
	Public Class CookieManager
		Inherits CookieHandler

		' ---------------- Fields -------------- 

		Private policyCallback As CookiePolicy


		Private cookieJar As CookieStore = Nothing


		' ---------------- Ctors -------------- 

		''' <summary>
		''' Create a new cookie manager.
		''' 
		''' <p>This constructor will create new cookie manager with default
		''' cookie store and accept policy. The effect is same as
		''' {@code CookieManager(null, null)}.
		''' </summary>
		Public Sub New()
			Me.New(Nothing, Nothing)
		End Sub


		''' <summary>
		''' Create a new cookie manager with specified cookie store and cookie policy.
		''' </summary>
		''' <param name="store">     a {@code CookieStore} to be used by cookie manager.
		'''                  if {@code null}, cookie manager will use a default one,
		'''                  which is an in-memory CookieStore implementation. </param>
		''' <param name="cookiePolicy">      a {@code CookiePolicy} instance
		'''                          to be used by cookie manager as policy callback.
		'''                          if {@code null}, ACCEPT_ORIGINAL_SERVER will
		'''                          be used. </param>
		Public Sub New(  store As CookieStore,   cookiePolicy As CookiePolicy)
			' use default cookie policy if not specify one
			policyCallback = If(cookiePolicy Is Nothing, CookiePolicy.ACCEPT_ORIGINAL_SERVER, cookiePolicy)

			' if not specify CookieStore to use, use default one
			If store Is Nothing Then
				cookieJar = New InMemoryCookieStore
			Else
				cookieJar = store
			End If
		End Sub


		' ---------------- Public operations -------------- 

		''' <summary>
		''' To set the cookie policy of this cookie manager.
		''' 
		''' <p> A instance of {@code CookieManager} will have
		''' cookie policy ACCEPT_ORIGINAL_SERVER by default. Users always
		''' can call this method to set another cookie policy.
		''' </summary>
		''' <param name="cookiePolicy">      the cookie policy. Can be {@code null}, which
		'''                          has no effects on current cookie policy. </param>
		Public Overridable Property cookiePolicy As CookiePolicy
			Set(  cookiePolicy As CookiePolicy)
				If cookiePolicy IsNot Nothing Then policyCallback = cookiePolicy
			End Set
		End Property


		''' <summary>
		''' To retrieve current cookie store.
		''' </summary>
		''' <returns>  the cookie store currently used by cookie manager. </returns>
		Public Overridable Property cookieStore As CookieStore
			Get
				Return cookieJar
			End Get
		End Property


		Public Overrides Function [get](  uri As URI,   requestHeaders As IDictionary(Of String, IList(Of String))) As IDictionary(Of String, IList(Of String))
			' pre-condition check
			If uri Is Nothing OrElse requestHeaders Is Nothing Then Throw New IllegalArgumentException("Argument is null")

			Dim cookieMap As IDictionary(Of String, IList(Of String)) = New Dictionary(Of String, IList(Of String))
			' if there's no default CookieStore, no way for us to get any cookie
			If cookieJar Is Nothing Then Return java.util.Collections.unmodifiableMap(cookieMap)

			Dim secureLink As Boolean = "https".equalsIgnoreCase(uri.scheme)
			Dim cookies As IList(Of HttpCookie) = New List(Of HttpCookie)
			Dim path As String = uri.path
			If path Is Nothing OrElse path.empty Then path = "/"
			For Each cookie As HttpCookie In cookieJar.get(uri)
				' apply path-matches rule (RFC 2965 sec. 3.3.4)
				' and check for the possible "secure" tag (i.e. don't send
				' 'secure' cookies over unsecure links)
				If pathMatches(path, cookie.path) AndAlso (secureLink OrElse (Not cookie.secure)) Then
					' Enforce httponly attribute
					If cookie.httpOnly Then
						Dim s As String = uri.scheme
						If (Not "http".equalsIgnoreCase(s)) AndAlso (Not "https".equalsIgnoreCase(s)) Then Continue For
					End If
					' Let's check the authorize port list if it exists
					Dim ports As String = cookie.portlist
					If ports IsNot Nothing AndAlso (Not ports.empty) Then
						Dim port As Integer = uri.port
						If port = -1 Then port = If("https".Equals(uri.scheme), 443, 80)
						If isInPortList(ports, port) Then cookies.Add(cookie)
					Else
						cookies.Add(cookie)
					End If
				End If
			Next cookie

			' apply sort rule (RFC 2965 sec. 3.3.4)
			Dim cookieHeader As IList(Of String) = sortByPath(cookies)

			cookieMap("Cookie") = cookieHeader
			Return java.util.Collections.unmodifiableMap(cookieMap)
		End Function

		Public Overrides Sub put(  uri As URI,   responseHeaders As IDictionary(Of String, IList(Of String)))
			' pre-condition check
			If uri Is Nothing OrElse responseHeaders Is Nothing Then Throw New IllegalArgumentException("Argument is null")


			' if there's no default CookieStore, no need to remember any cookie
			If cookieJar Is Nothing Then Return

		Dim logger As sun.util.logging.PlatformLogger = sun.util.logging.PlatformLogger.getLogger("java.net.CookieManager")
			For Each headerKey As String In responseHeaders.Keys
				' RFC 2965 3.2.2, key must be 'Set-Cookie2'
				' we also accept 'Set-Cookie' here for backward compatibility
				If headerKey Is Nothing OrElse Not(headerKey.equalsIgnoreCase("Set-Cookie2") OrElse headerKey.equalsIgnoreCase("Set-Cookie")) Then Continue For

				For Each headerValue As String In responseHeaders(headerKey)
					Try
						Dim cookies As IList(Of HttpCookie)
						Try
							cookies = HttpCookie.parse(headerValue)
						Catch e As IllegalArgumentException
							' Bogus header, make an empty list and log the error
							cookies = java.util.Collections.emptyList()
							If logger.isLoggable(sun.util.logging.PlatformLogger.Level.SEVERE) Then logger.severe("Invalid cookie for " & uri & ": " & headerValue)
						End Try
						For Each cookie As HttpCookie In cookies
							If cookie.path Is Nothing Then
								' If no path is specified, then by default
								' the path is the directory of the page/doc
								Dim path As String = uri.path
								If Not path.EndsWith("/") Then
									Dim i As Integer = path.LastIndexOf("/")
									If i > 0 Then
										path = path.Substring(0, i + 1)
									Else
										path = "/"
									End If
								End If
								cookie.path = path
							End If

							' As per RFC 2965, section 3.3.1:
							' Domain  Defaults to the effective request-host.  (Note that because
							' there is no dot at the beginning of effective request-host,
							' the default Domain can only domain-match itself.)
							If cookie.domain Is Nothing Then
								Dim host As String = uri.host
								If host IsNot Nothing AndAlso (Not host.contains(".")) Then host &= ".local"
								cookie.domain = host
							End If
							Dim ports As String = cookie.portlist
							If ports IsNot Nothing Then
								Dim port As Integer = uri.port
								If port = -1 Then port = If("https".Equals(uri.scheme), 443, 80)
								If ports.empty Then
									' Empty port list means this should be restricted
									' to the incoming URI port
									cookie.portlist = "" & port
									If shouldAcceptInternal(uri, cookie) Then cookieJar.add(uri, cookie)
								Else
									' Only store cookies with a port list
									' IF the URI port is in that list, as per
									' RFC 2965 section 3.3.2
									If isInPortList(ports, port) AndAlso shouldAcceptInternal(uri, cookie) Then cookieJar.add(uri, cookie)
								End If
							Else
								If shouldAcceptInternal(uri, cookie) Then cookieJar.add(uri, cookie)
							End If
						Next cookie
					Catch e As IllegalArgumentException
						' invalid set-cookie header string
						' no-op
					End Try
				Next headerValue
			Next headerKey
		End Sub


		' ---------------- Private operations -------------- 

		' to determine whether or not accept this cookie
		Private Function shouldAcceptInternal(  uri As URI,   cookie As HttpCookie) As Boolean
			Try
				Return policyCallback.shouldAccept(uri, cookie) ' pretect against malicious callback
			Catch ignored As Exception
				Return False
			End Try
		End Function


		Private Shared Function isInPortList(  lst As String,   port As Integer) As Boolean
			Dim i As Integer = lst.IndexOf(",")
			Dim val As Integer = -1
			Do While i > 0
				Try
					val = Convert.ToInt32(lst.Substring(0, i))
					If val = port Then Return True
				Catch numberFormatException_Renamed As NumberFormatException
				End Try
				lst = lst.Substring(i+1)
				i = lst.IndexOf(",")
			Loop
			If Not lst.empty Then
				Try
					val = Convert.ToInt32(lst)
					If val = port Then Return True
				Catch numberFormatException_Renamed As NumberFormatException
				End Try
			End If
			Return False
		End Function

	'    
	'     * path-matches algorithm, as defined by RFC 2965
	'     
		Private Function pathMatches(  path As String,   pathToMatchWith As String) As Boolean
			If path = pathToMatchWith Then Return True
			If path Is Nothing OrElse pathToMatchWith Is Nothing Then Return False
			If path.StartsWith(pathToMatchWith) Then Return True

			Return False
		End Function


	'    
	'     * sort cookies with respect to their path: those with more specific Path attributes
	'     * precede those with less specific, as defined in RFC 2965 sec. 3.3.4
	'     
		Private Function sortByPath(  cookies As IList(Of HttpCookie)) As IList(Of String)
			java.util.Collections.sort(cookies, New CookiePathComparator)

			Dim cookieHeader As IList(Of String) = New List(Of String)
			For Each cookie As HttpCookie In cookies
				' Netscape cookie spec and RFC 2965 have different format of Cookie
				' header; RFC 2965 requires a leading $Version="1" string while Netscape
				' does not.
				' The workaround here is to add a $Version="1" string in advance
				If cookies.IndexOf(cookie) = 0 AndAlso cookie.version > 0 Then cookieHeader.Add("$Version=""1""")

				cookieHeader.Add(cookie.ToString())
			Next cookie
			Return cookieHeader
		End Function


		Friend Class CookiePathComparator
			Implements IComparer(Of HttpCookie)

			Public Overridable Function compare(  c1 As HttpCookie,   c2 As HttpCookie) As Integer
				If c1 Is c2 Then Return 0
				If c1 Is Nothing Then Return -1
				If c2 Is Nothing Then Return 1

				' path rule only applies to the cookies with same name
				If Not c1.name.Equals(c2.name) Then Return 0

				' those with more specific Path attributes precede those with less specific
				If c1.path.StartsWith(c2.path) Then
					Return -1
				ElseIf c2.path.StartsWith(c1.path) Then
					Return 1
				Else
					Return 0
				End If
			End Function
		End Class
	End Class

End Namespace