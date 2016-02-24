Imports System.Runtime.CompilerServices
Imports System.Collections.Generic

'
' * Copyright (c) 2003, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' A CookieHandler object provides a callback mechanism to hook up a
	''' HTTP state management policy implementation into the HTTP protocol
	''' handler. The HTTP state management mechanism specifies a way to
	''' create a stateful session with HTTP requests and responses.
	''' 
	''' <p>A system-wide CookieHandler that to used by the HTTP protocol
	''' handler can be registered by doing a
	''' CookieHandler.setDefault(CookieHandler). The currently registered
	''' CookieHandler can be retrieved by calling
	''' CookieHandler.getDefault().
	''' 
	''' For more information on HTTP state management, see <a
	''' href="http://www.ietf.org/rfc/rfc2965.txt"><i>RFC&nbsp;2965: HTTP
	''' State Management Mechanism</i></a>
	''' 
	''' @author Yingxian Wang
	''' @since 1.5
	''' </summary>
	Public MustInherit Class CookieHandler
		''' <summary>
		''' The system-wide cookie handler that will apply cookies to the
		''' request headers and manage cookies from the response headers.
		''' </summary>
		''' <seealso cref= setDefault(CookieHandler) </seealso>
		''' <seealso cref= getDefault() </seealso>
		Private Shared cookieHandler As CookieHandler

		''' <summary>
		''' Gets the system-wide cookie handler.
		''' </summary>
		''' <returns> the system-wide cookie handler; A null return means
		'''        there is no system-wide cookie handler currently set. </returns>
		''' <exception cref="SecurityException">
		'''       If a security manager has been installed and it denies
		''' <seealso cref="NetPermission"/>{@code ("getCookieHandler")} </exception>
		''' <seealso cref= #setDefault(CookieHandler) </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Property Shared [default] As CookieHandler
			Get
				Dim sm As SecurityManager = System.securityManager
				If sm IsNot Nothing Then sm.checkPermission(sun.security.util.SecurityConstants.GET_COOKIEHANDLER_PERMISSION)
				Return cookieHandler
			End Get
			Set(ByVal cHandler As CookieHandler)
				Dim sm As SecurityManager = System.securityManager
				If sm IsNot Nothing Then sm.checkPermission(sun.security.util.SecurityConstants.SET_COOKIEHANDLER_PERMISSION)
				cookieHandler = cHandler
			End Set
		End Property


		''' <summary>
		''' Gets all the applicable cookies from a cookie cache for the
		''' specified uri in the request header.
		''' 
		''' <P>The {@code URI} passed as an argument specifies the intended use for
		''' the cookies. In particular the scheme should reflect whether the cookies
		''' will be sent over http, https or used in another context like javascript.
		''' The host part should reflect either the destination of the cookies or
		''' their origin in the case of javascript.</P>
		''' <P>It is up to the implementation to take into account the {@code URI} and
		''' the cookies attributes and security settings to determine which ones
		''' should be returned.</P>
		''' 
		''' <P>HTTP protocol implementers should make sure that this method is
		''' called after all request headers related to choosing cookies
		''' are added, and before the request is sent.</P>
		''' </summary>
		''' <param name="uri"> a {@code URI} representing the intended use for the
		'''            cookies </param>
		''' <param name="requestHeaders"> - a Map from request header
		'''            field names to lists of field values representing
		'''            the current request headers </param>
		''' <returns> an immutable map from state management headers, with
		'''            field names "Cookie" or "Cookie2" to a list of
		'''            cookies containing state information
		''' </returns>
		''' <exception cref="IOException"> if an I/O error occurs </exception>
		''' <exception cref="IllegalArgumentException"> if either argument is null </exception>
		''' <seealso cref= #put(URI, Map) </seealso>
		Public MustOverride Function [get](ByVal uri As URI, ByVal requestHeaders As IDictionary(Of String, IList(Of String))) As IDictionary(Of String, IList(Of String))

		''' <summary>
		''' Sets all the applicable cookies, examples are response header
		''' fields that are named Set-Cookie2, present in the response
		''' headers into a cookie cache.
		''' </summary>
		''' <param name="uri"> a {@code URI} where the cookies come from </param>
		''' <param name="responseHeaders"> an immutable map from field names to
		'''            lists of field values representing the response
		'''            header fields returned </param>
		''' <exception cref="IOException"> if an I/O error occurs </exception>
		''' <exception cref="IllegalArgumentException"> if either argument is null </exception>
		''' <seealso cref= #get(URI, Map) </seealso>
		Public MustOverride Sub put(ByVal uri As URI, ByVal responseHeaders As IDictionary(Of String, IList(Of String)))
	End Class

End Namespace