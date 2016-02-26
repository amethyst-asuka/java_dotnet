Imports System.Collections.Generic

'
' * Copyright (c) 2005, 2010, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.xml.ws.spi.http


	''' <summary>
	''' This class encapsulates a HTTP request received and a
	''' response to be generated in one exchange. It provides methods
	''' for examining the request from the client, and for building and
	''' sending the response.
	''' <p>
	''' A <code>HttpExchange</code> must be closed to free or reuse
	''' underlying resources. The effect of failing to close an exchange
	''' is undefined.
	''' 
	''' @author Jitendra Kotamraju
	''' @since JAX-WS 2.2
	''' </summary>
	Public MustInherit Class HttpExchange

		''' <summary>
		''' Standard property: cipher suite value when the request is received
		''' over HTTPS
		''' <p>Type: String
		''' </summary>
		Public Const REQUEST_CIPHER_SUITE As String = "javax.xml.ws.spi.http.request.cipher.suite"

		''' <summary>
		''' Standard property: bit size of the algorithm when the request is
		''' received over HTTPS
		''' <p>Type: Integer
		''' </summary>
		Public Const REQUEST_KEY_SIZE As String = "javax.xml.ws.spi.http.request.key.size"

		''' <summary>
		''' Standard property: A SSL certificate, if any, associated with the request
		''' 
		''' <p>Type: java.security.cert.X509Certificate[]
		''' The order of this array is defined as being in ascending order of trust.
		''' The first certificate in the chain is the one set by the client, the next
		''' is the one used to authenticate the first, and so on.
		''' </summary>
		Public Const REQUEST_X509CERTIFICATE As String = "javax.xml.ws.spi.http.request.cert.X509Certificate"

		''' <summary>
		''' Returns an immutable Map containing the HTTP headers that were
		''' included with this request. The keys in this Map will be the header
		''' names, while the values will be a List of Strings containing each value
		''' that was included (either for a header that was listed several times,
		''' or one that accepts a comma-delimited list of values on a single line).
		''' In either of these cases, the values for the header name will be
		''' presented in the order that they were included in the request.
		''' <p>
		''' The keys in Map are case-insensitive.
		''' </summary>
		''' <returns> an immutable Map which can be used to access request headers </returns>
		Public MustOverride ReadOnly Property requestHeaders As IDictionary(Of String, IList(Of String))

		''' <summary>
		''' Returns the value of the specified request header. If the request
		''' did not include a header of the specified name, this method returns
		''' null. If there are multiple headers with the same name, this method
		''' returns the first header in the request. The header name is
		''' case-insensitive. This is a convienence method to get a header
		''' (instead of using the <seealso cref="#getRequestHeaders"/>).
		''' </summary>
		''' <param name="name"> the name of the request header </param>
		''' <returns> returns the value of the requested header,
		'''         or null if the request does not have a header of that name </returns>
		 Public MustOverride Function getRequestHeader(ByVal name As String) As String

		''' <summary>
		''' Returns a mutable Map into which the HTTP response headers can be stored
		''' and which will be transmitted as part of this response. The keys in the
		''' Map will be the header names, while the values must be a List of Strings
		''' containing each value that should be included multiple times
		''' (in the order that they should be included).
		''' <p>
		''' The keys in Map are case-insensitive.
		''' </summary>
		''' <returns> a mutable Map which can be used to set response headers. </returns>
		Public MustOverride ReadOnly Property responseHeaders As IDictionary(Of String, IList(Of String))

		''' <summary>
		''' Adds a response header with the given name and value. This method
		''' allows a response header to have multiple values. This is a
		''' convenience method to add a response header(instead of using the
		''' <seealso cref="#getResponseHeaders()"/>).
		''' </summary>
		''' <param name="name"> the name of the header </param>
		''' <param name="value"> the additional header value. If it contains octet string,
		'''        it should be encoded according to
		'''        RFC 2047 (http://www.ietf.org/rfc/rfc2047.txt)
		''' </param>
		''' <seealso cref= #getResponseHeaders </seealso>
		Public MustOverride Sub addResponseHeader(ByVal name As String, ByVal value As String)

		''' <summary>
		''' Returns the part of the request's URI from the protocol
		''' name up to the query string in the first line of the HTTP request.
		''' Container doesn't decode this string.
		''' </summary>
		''' <returns> the request URI </returns>
		Public MustOverride ReadOnly Property requestURI As String

		''' <summary>
		''' Returns the context path of all the endpoints in an application.
		''' This path is the portion of the request URI that indicates the
		''' context of the request. The context path always comes first in a
		''' request URI. The path starts with a "/" character but does not
		''' end with a "/" character. If this method returns "", the request
		''' is for default context. The container does not decode this string.
		''' 
		''' <p>
		''' Context path is used in computing the endpoint address. See
		''' <seealso cref="HttpContext#getPath"/>
		''' </summary>
		''' <returns> context path of all the endpoints in an application </returns>
		''' <seealso cref= HttpContext#getPath </seealso>
		Public MustOverride ReadOnly Property contextPath As String

		''' <summary>
		''' Get the HTTP request method
		''' </summary>
		''' <returns> the request method </returns>
		Public MustOverride ReadOnly Property requestMethod As String

		''' <summary>
		''' Returns a <seealso cref="HttpContext"/> for this exchange.
		''' Container matches the request with the associated Endpoint's HttpContext
		''' </summary>
		''' <returns> the HttpContext for this exchange </returns>
		Public MustOverride ReadOnly Property httpContext As HttpContext

		''' <summary>
		''' This must be called to end an exchange. Container takes care of
		''' closing request and response streams. This must be called so that
		''' the container can free or reuse underlying resources.
		''' </summary>
		''' <exception cref="IOException"> if any i/o error </exception>
		Public MustOverride Sub close()

		''' <summary>
		''' Returns a stream from which the request body can be read.
		''' Multiple calls to this method will return the same stream.
		''' </summary>
		''' <returns> the stream from which the request body can be read. </returns>
		''' <exception cref="IOException"> if any i/o error during request processing </exception>
		Public MustOverride ReadOnly Property requestBody As java.io.InputStream

		''' <summary>
		''' Returns a stream to which the response body must be
		''' written. <seealso cref="#setStatus"/>) must be called prior to calling
		''' this method. Multiple calls to this method (for the same exchange)
		''' will return the same stream.
		''' </summary>
		''' <returns> the stream to which the response body is written </returns>
		''' <exception cref="IOException"> if any i/o error during response processing </exception>
		Public MustOverride ReadOnly Property responseBody As java.io.OutputStream

		''' <summary>
		''' Sets the HTTP status code for the response.
		''' 
		''' <p>
		''' This method must be called prior to calling <seealso cref="#getResponseBody"/>.
		''' </summary>
		''' <param name="status"> the response code to send </param>
		''' <seealso cref= #getResponseBody </seealso>
		Public MustOverride WriteOnly Property status As Integer

		''' <summary>
		''' Returns the unresolved address of the remote entity invoking
		''' this request.
		''' </summary>
		''' <returns> the InetSocketAddress of the caller </returns>
		Public MustOverride ReadOnly Property remoteAddress As java.net.InetSocketAddress

		''' <summary>
		''' Returns the unresolved local address on which the request was received.
		''' </summary>
		''' <returns> the InetSocketAddress of the local interface </returns>
		Public MustOverride ReadOnly Property localAddress As java.net.InetSocketAddress

		''' <summary>
		''' Returns the protocol string from the request in the form
		''' <i>protocol/majorVersion.minorVersion</i>. For example,
		''' "HTTP/1.1"
		''' </summary>
		''' <returns> the protocol string from the request </returns>
		Public MustOverride ReadOnly Property protocol As String

		''' <summary>
		''' Returns the name of the scheme used to make this request,
		''' for example: http, or https.
		''' </summary>
		''' <returns> name of the scheme used to make this request </returns>
		Public MustOverride ReadOnly Property scheme As String

		''' <summary>
		''' Returns the extra path information that follows the web service
		''' path but precedes the query string in the request URI and will start
		''' with a "/" character.
		''' 
		''' <p>
		''' This can be used for <seealso cref="MessageContext#PATH_INFO"/>
		''' </summary>
		''' <returns> decoded extra path information of web service.
		'''         It is the path that comes
		'''         after the web service path but before the query string in the
		'''         request URI
		'''         <tt>null</tt> if there is no extra path in the request URI </returns>
		Public MustOverride ReadOnly Property pathInfo As String

		''' <summary>
		''' Returns the query string that is contained in the request URI
		''' after the path.
		''' 
		''' <p>
		''' This can be used for <seealso cref="MessageContext#QUERY_STRING"/>
		''' </summary>
		''' <returns> undecoded query string of request URI, or
		'''         <tt>null</tt> if the request URI doesn't have one </returns>
		Public MustOverride ReadOnly Property queryString As String

		''' <summary>
		''' Returns an attribute that is associated with this
		''' <code>HttpExchange</code>. JAX-WS handlers and endpoints may then
		''' access the attribute via <seealso cref="MessageContext"/>.
		''' <p>
		''' Servlet containers must expose <seealso cref="MessageContext#SERVLET_CONTEXT"/>,
		''' <seealso cref="MessageContext#SERVLET_REQUEST"/>, and
		''' <seealso cref="MessageContext#SERVLET_RESPONSE"/>
		''' as attributes.
		''' 
		''' <p>If the request has been received by the container using HTTPS, the
		''' following information must be exposed as attributes. These attributes
		''' are <seealso cref="#REQUEST_CIPHER_SUITE"/>, and <seealso cref="#REQUEST_KEY_SIZE"/>.
		''' If there is a SSL certificate associated with the request, it must
		''' be exposed using <seealso cref="#REQUEST_X509CERTIFICATE"/>
		''' </summary>
		''' <param name="name"> attribute name </param>
		''' <returns> the attribute value, or <tt>null</tt> if the attribute doesn't
		'''         exist </returns>
		Public MustOverride Function getAttribute(ByVal name As String) As Object

		''' <summary>
		''' Gives all the attribute names that are associated with
		''' this <code>HttpExchange</code>.
		''' </summary>
		''' <returns> set of all attribute names </returns>
		''' <seealso cref= #getAttribute(String) </seealso>
		Public MustOverride ReadOnly Property attributeNames As java.util.Set(Of String)

		''' <summary>
		''' Returns the <seealso cref="Principal"/> that represents the authenticated
		''' user for this <code>HttpExchange</code>.
		''' </summary>
		''' <returns> Principal for an authenticated user, or
		'''         <tt>null</tt> if not authenticated </returns>
		Public MustOverride ReadOnly Property userPrincipal As java.security.Principal

		''' <summary>
		''' Indicates whether an authenticated user is included in the specified
		''' logical "role".
		''' </summary>
		''' <param name="role"> specifies the name of the role </param>
		''' <returns> <tt>true</tt> if the user making this request belongs to a
		'''         given role </returns>
		Public MustOverride Function isUserInRole(ByVal role As String) As Boolean

	End Class

End Namespace