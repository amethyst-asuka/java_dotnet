Imports System

'
' * Copyright (c) 1996, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' A URLConnection with support for HTTP-specific features. See
	''' <A HREF="http://www.w3.org/pub/WWW/Protocols/"> the spec </A> for
	''' details.
	''' <p>
	''' 
	''' Each HttpURLConnection instance is used to make a single request
	''' but the underlying network connection to the HTTP server may be
	''' transparently shared by other instances. Calling the close() methods
	''' on the InputStream or OutputStream of an HttpURLConnection
	''' after a request may free network resources associated with this
	''' instance but has no effect on any shared persistent connection.
	''' Calling the disconnect() method may close the underlying socket
	''' if a persistent connection is otherwise idle at that time.
	''' 
	''' <P>The HTTP protocol handler has a few settings that can be accessed through
	''' System Properties. This covers
	''' <a href="doc-files/net-properties.html#Proxies">Proxy settings</a> as well as
	''' <a href="doc-files/net-properties.html#MiscHTTP"> various other settings</a>.
	''' </P>
	''' <p>
	''' <b>Security permissions</b>
	''' <p>
	''' If a security manager is installed, and if a method is called which results in an
	''' attempt to open a connection, the caller must possess either:-
	''' <ul><li>a "connect" <seealso cref="SocketPermission"/> to the host/port combination of the
	''' destination URL or</li>
	''' <li>a <seealso cref="URLPermission"/> that permits this request.</li>
	''' </ul><p>
	''' If automatic redirection is enabled, and this request is redirected to another
	''' destination, then the caller must also have permission to connect to the
	''' redirected host/URL.
	''' </summary>
	''' <seealso cref=     java.net.HttpURLConnection#disconnect()
	''' @since JDK1.1 </seealso>
	Public MustInherit Class HttpURLConnection
		Inherits URLConnection

		' instance variables 

		''' <summary>
		''' The HTTP method (GET,POST,PUT,etc.).
		''' </summary>
		Protected Friend method As String = "GET"

		''' <summary>
		''' The chunk-length when using chunked encoding streaming mode for output.
		''' A value of {@code -1} means chunked encoding is disabled for output.
		''' @since 1.5
		''' </summary>
		Protected Friend chunkLength As Integer = -1

		''' <summary>
		''' The fixed content-length when using fixed-length streaming mode.
		''' A value of {@code -1} means fixed-length streaming mode is disabled
		''' for output.
		''' 
		''' <P> <B>NOTE:</B> <seealso cref="#fixedContentLengthLong"/> is recommended instead
		''' of this field, as it allows larger content lengths to be set.
		''' 
		''' @since 1.5
		''' </summary>
		Protected Friend fixedContentLength As Integer = -1

		''' <summary>
		''' The fixed content-length when using fixed-length streaming mode.
		''' A value of {@code -1} means fixed-length streaming mode is disabled
		''' for output.
		''' 
		''' @since 1.7
		''' </summary>
		Protected Friend fixedContentLengthLong As Long = -1

		''' <summary>
		''' Returns the key for the {@code n}<sup>th</sup> header field.
		''' Some implementations may treat the {@code 0}<sup>th</sup>
		''' header field as special, i.e. as the status line returned by the HTTP
		''' server. In this case, <seealso cref="#getHeaderField(int) getHeaderField(0)"/> returns the status
		''' line, but {@code getHeaderFieldKey(0)} returns null.
		''' </summary>
		''' <param name="n">   an index, where {@code n >=0}. </param>
		''' <returns>  the key for the {@code n}<sup>th</sup> header field,
		'''          or {@code null} if the key does not exist. </returns>
		Public Overrides Function getHeaderFieldKey(ByVal n As Integer) As String
			Return Nothing
		End Function

		''' <summary>
		''' This method is used to enable streaming of a HTTP request body
		''' without internal buffering, when the content length is known in
		''' advance.
		''' <p>
		''' An exception will be thrown if the application
		''' attempts to write more data than the indicated
		''' content-length, or if the application closes the OutputStream
		''' before writing the indicated amount.
		''' <p>
		''' When output streaming is enabled, authentication
		''' and redirection cannot be handled automatically.
		''' A HttpRetryException will be thrown when reading
		''' the response if authentication or redirection are required.
		''' This exception can be queried for the details of the error.
		''' <p>
		''' This method must be called before the URLConnection is connected.
		''' <p>
		''' <B>NOTE:</B> <seealso cref="#setFixedLengthStreamingMode(long)"/> is recommended
		''' instead of this method as it allows larger content lengths to be set.
		''' </summary>
		''' <param name="contentLength"> The number of bytes which will be written
		'''          to the OutputStream.
		''' </param>
		''' <exception cref="IllegalStateException"> if URLConnection is already connected
		'''          or if a different streaming mode is already enabled.
		''' </exception>
		''' <exception cref="IllegalArgumentException"> if a content length less than
		'''          zero is specified.
		''' </exception>
		''' <seealso cref=     #setChunkedStreamingMode(int)
		''' @since 1.5 </seealso>
		Public Overridable Property fixedLengthStreamingMode As Integer
			Set(ByVal contentLength As Integer)
				If connected Then Throw New IllegalStateException("Already connected")
				If chunkLength <> -1 Then Throw New IllegalStateException("Chunked encoding streaming mode set")
				If contentLength < 0 Then Throw New IllegalArgumentException("invalid content length")
				fixedContentLength = contentLength
			End Set
		End Property

		''' <summary>
		''' This method is used to enable streaming of a HTTP request body
		''' without internal buffering, when the content length is known in
		''' advance.
		''' 
		''' <P> An exception will be thrown if the application attempts to write
		''' more data than the indicated content-length, or if the application
		''' closes the OutputStream before writing the indicated amount.
		''' 
		''' <P> When output streaming is enabled, authentication and redirection
		''' cannot be handled automatically. A <seealso cref="HttpRetryException"/> will
		''' be thrown when reading the response if authentication or redirection
		''' are required. This exception can be queried for the details of the
		''' error.
		''' 
		''' <P> This method must be called before the URLConnection is connected.
		''' 
		''' <P> The content length set by invoking this method takes precedence
		''' over any value set by <seealso cref="#setFixedLengthStreamingMode(int)"/>.
		''' </summary>
		''' <param name="contentLength">
		'''         The number of bytes which will be written to the OutputStream.
		''' </param>
		''' <exception cref="IllegalStateException">
		'''          if URLConnection is already connected or if a different
		'''          streaming mode is already enabled.
		''' </exception>
		''' <exception cref="IllegalArgumentException">
		'''          if a content length less than zero is specified.
		''' 
		''' @since 1.7 </exception>
		Public Overridable Property fixedLengthStreamingMode As Long
			Set(ByVal contentLength As Long)
				If connected Then Throw New IllegalStateException("Already connected")
				If chunkLength <> -1 Then Throw New IllegalStateException("Chunked encoding streaming mode set")
				If contentLength < 0 Then Throw New IllegalArgumentException("invalid content length")
				fixedContentLengthLong = contentLength
			End Set
		End Property

	'     Default chunk size (including chunk header) if not specified;
	'     * we want to keep this in sync with the one defined in
	'     * sun.net.www.http.ChunkedOutputStream
	'     
		Private Const DEFAULT_CHUNK_SIZE As Integer = 4096

		''' <summary>
		''' This method is used to enable streaming of a HTTP request body
		''' without internal buffering, when the content length is <b>not</b>
		''' known in advance. In this mode, chunked transfer encoding
		''' is used to send the request body. Note, not all HTTP servers
		''' support this mode.
		''' <p>
		''' When output streaming is enabled, authentication
		''' and redirection cannot be handled automatically.
		''' A HttpRetryException will be thrown when reading
		''' the response if authentication or redirection are required.
		''' This exception can be queried for the details of the error.
		''' <p>
		''' This method must be called before the URLConnection is connected.
		''' </summary>
		''' <param name="chunklen"> The number of bytes to write in each chunk.
		'''          If chunklen is less than or equal to zero, a default
		'''          value will be used.
		''' </param>
		''' <exception cref="IllegalStateException"> if URLConnection is already connected
		'''          or if a different streaming mode is already enabled.
		''' </exception>
		''' <seealso cref=     #setFixedLengthStreamingMode(int)
		''' @since 1.5 </seealso>
		Public Overridable Property chunkedStreamingMode As Integer
			Set(ByVal chunklen As Integer)
				If connected Then Throw New IllegalStateException("Can't set streaming mode: already connected")
				If fixedContentLength <> -1 OrElse fixedContentLengthLong <> -1 Then Throw New IllegalStateException("Fixed length streaming mode set")
				chunkLength = If(chunklen <=0, DEFAULT_CHUNK_SIZE, chunklen)
			End Set
		End Property

		''' <summary>
		''' Returns the value for the {@code n}<sup>th</sup> header field.
		''' Some implementations may treat the {@code 0}<sup>th</sup>
		''' header field as special, i.e. as the status line returned by the HTTP
		''' server.
		''' <p>
		''' This method can be used in conjunction with the
		''' <seealso cref="#getHeaderFieldKey getHeaderFieldKey"/> method to iterate through all
		''' the headers in the message.
		''' </summary>
		''' <param name="n">   an index, where {@code n>=0}. </param>
		''' <returns>  the value of the {@code n}<sup>th</sup> header field,
		'''          or {@code null} if the value does not exist. </returns>
		''' <seealso cref=     java.net.HttpURLConnection#getHeaderFieldKey(int) </seealso>
		Public Overrides Function getHeaderField(ByVal n As Integer) As String
			Return Nothing
		End Function

		''' <summary>
		''' An {@code int} representing the three digit HTTP Status-Code.
		''' <ul>
		''' <li> 1xx: Informational
		''' <li> 2xx: Success
		''' <li> 3xx: Redirection
		''' <li> 4xx: Client Error
		''' <li> 5xx: Server Error
		''' </ul>
		''' </summary>
		Protected Friend responseCode As Integer = -1

		''' <summary>
		''' The HTTP response message.
		''' </summary>
		Protected Friend responseMessage As String = Nothing

		' static variables 

		' do we automatically follow redirects? The default is true. 
		Private Shared followRedirects As Boolean = True

		''' <summary>
		''' If {@code true}, the protocol will automatically follow redirects.
		''' If {@code false}, the protocol will not automatically follow
		''' redirects.
		''' <p>
		''' This field is set by the {@code setInstanceFollowRedirects}
		''' method. Its value is returned by the {@code getInstanceFollowRedirects}
		''' method.
		''' <p>
		''' Its default value is based on the value of the static followRedirects
		''' at HttpURLConnection construction time.
		''' </summary>
		''' <seealso cref=     java.net.HttpURLConnection#setInstanceFollowRedirects(boolean) </seealso>
		''' <seealso cref=     java.net.HttpURLConnection#getInstanceFollowRedirects() </seealso>
		''' <seealso cref=     java.net.HttpURLConnection#setFollowRedirects(boolean) </seealso>
		Protected Friend instanceFollowRedirects As Boolean = followRedirects

		' valid HTTP methods 
		Private Shared ReadOnly methods As String() = { "GET", "POST", "HEAD", "OPTIONS", "PUT", "DELETE", "TRACE" }

		''' <summary>
		''' Constructor for the HttpURLConnection. </summary>
		''' <param name="u"> the URL </param>
		Protected Friend Sub New(ByVal u As URL)
			MyBase.New(u)
		End Sub

		''' <summary>
		''' Sets whether HTTP redirects  (requests with response code 3xx) should
		''' be automatically followed by this class.  True by default.  Applets
		''' cannot change this variable.
		''' <p>
		''' If there is a security manager, this method first calls
		''' the security manager's {@code checkSetFactory} method
		''' to ensure the operation is allowed.
		''' This could result in a SecurityException.
		''' </summary>
		''' <param name="set"> a {@code boolean} indicating whether or not
		''' to follow HTTP redirects. </param>
		''' <exception cref="SecurityException">  if a security manager exists and its
		'''             {@code checkSetFactory} method doesn't
		'''             allow the operation. </exception>
		''' <seealso cref=        SecurityManager#checkSetFactory </seealso>
		''' <seealso cref= #getFollowRedirects() </seealso>
		Public Shared Property followRedirects As Boolean
			Set(ByVal [set] As Boolean)
				Dim sec As SecurityManager = System.securityManager
				If sec IsNot Nothing Then sec.checkSetFactory()
				followRedirects = [set]
			End Set
			Get
				Return followRedirects
			End Get
		End Property


		''' <summary>
		''' Sets whether HTTP redirects (requests with response code 3xx) should
		''' be automatically followed by this {@code HttpURLConnection}
		''' instance.
		''' <p>
		''' The default value comes from followRedirects, which defaults to
		''' true.
		''' </summary>
		''' <param name="followRedirects"> a {@code boolean} indicating
		''' whether or not to follow HTTP redirects.
		''' </param>
		''' <seealso cref=    java.net.HttpURLConnection#instanceFollowRedirects </seealso>
		''' <seealso cref= #getInstanceFollowRedirects
		''' @since 1.3 </seealso>
		 Public Overridable Property instanceFollowRedirects As Boolean
			 Set(ByVal followRedirects As Boolean)
				instanceFollowRedirects = followRedirects
			 End Set
			 Get
				 Return instanceFollowRedirects
			 End Get
		 End Property


		''' <summary>
		''' Set the method for the URL request, one of:
		''' <UL>
		'''  <LI>GET
		'''  <LI>POST
		'''  <LI>HEAD
		'''  <LI>OPTIONS
		'''  <LI>PUT
		'''  <LI>DELETE
		'''  <LI>TRACE
		''' </UL> are legal, subject to protocol restrictions.  The default
		''' method is GET.
		''' </summary>
		''' <param name="method"> the HTTP method </param>
		''' <exception cref="ProtocolException"> if the method cannot be reset or if
		'''              the requested method isn't valid for HTTP. </exception>
		''' <exception cref="SecurityException"> if a security manager is set and the
		'''              method is "TRACE", but the "allowHttpTrace"
		'''              NetPermission is not granted. </exception>
		''' <seealso cref= #getRequestMethod() </seealso>
		Public Overridable Property requestMethod As String
			Set(ByVal method As String)
				If connected Then Throw New ProtocolException("Can't reset method: already connected")
				' This restriction will prevent people from using this class to
				' experiment w/ new HTTP methods using java.  But it should
				' be placed for security - the request String could be
				' arbitrarily java.lang.[Long].
    
				For i As Integer = 0 To methods.Length - 1
					If methods(i).Equals(method) Then
						If method.Equals("TRACE") Then
							Dim s As SecurityManager = System.securityManager
							If s IsNot Nothing Then s.checkPermission(New NetPermission("allowHttpTrace"))
						End If
						Me.method = method
						Return
					End If
				Next i
				Throw New ProtocolException("Invalid HTTP method: " & method)
			End Set
			Get
				Return method
			End Get
		End Property


		''' <summary>
		''' Gets the status code from an HTTP response message.
		''' For example, in the case of the following status lines:
		''' <PRE>
		''' HTTP/1.0 200 OK
		''' HTTP/1.0 401 Unauthorized
		''' </PRE>
		''' It will return 200 and 401 respectively.
		''' Returns -1 if no code can be discerned
		''' from the response (i.e., the response is not valid HTTP). </summary>
		''' <exception cref="IOException"> if an error occurred connecting to the server. </exception>
		''' <returns> the HTTP Status-Code, or -1 </returns>
		Public Overridable Property responseCode As Integer
			Get
		'        
		'         * We're got the response code already
		'         
				If responseCode <> -1 Then Return responseCode
    
		'        
		'         * Ensure that we have connected to the server. Record
		'         * exception as we need to re-throw it if there isn't
		'         * a status line.
		'         
				Dim exc As Exception = Nothing
				Try
					inputStream
				Catch e As Exception
					exc = e
				End Try
    
		'        
		'         * If we can't a status-line then re-throw any exception
		'         * that getInputStream threw.
		'         
				Dim statusLine As String = getHeaderField(0)
				If statusLine Is Nothing Then
					If exc IsNot Nothing Then
						If TypeOf exc Is RuntimeException Then
							Throw CType(exc, RuntimeException)
						Else
							Throw CType(exc, java.io.IOException)
						End If
					End If
					Return -1
				End If
    
		'        
		'         * Examine the status-line - should be formatted as per
		'         * section 6.1 of RFC 2616 :-
		'         *
		'         * Status-Line = HTTP-Version SP Status-Code SP Reason-Phrase
		'         *
		'         * If status line can't be parsed return -1.
		'         
				If statusLine.StartsWith("HTTP/1.") Then
					Dim codePos As Integer = statusLine.IndexOf(" "c)
					If codePos > 0 Then
    
						Dim phrasePos As Integer = statusLine.IndexOf(" "c, codePos+1)
						If phrasePos > 0 AndAlso phrasePos < statusLine.length() Then responseMessage = statusLine.Substring(phrasePos+1)
    
						' deviation from RFC 2616 - don't reject status line
						' if SP Reason-Phrase is not included.
						If phrasePos < 0 Then phrasePos = statusLine.length()
    
						Try
							responseCode = Convert.ToInt32(statusLine.Substring(codePos+1, phrasePos - (codePos+1)))
							Return responseCode
						Catch e As NumberFormatException
						End Try
					End If
				End If
				Return -1
			End Get
		End Property

		''' <summary>
		''' Gets the HTTP response message, if any, returned along with the
		''' response code from a server.  From responses like:
		''' <PRE>
		''' HTTP/1.0 200 OK
		''' HTTP/1.0 404 Not Found
		''' </PRE>
		''' Extracts the Strings "OK" and "Not Found" respectively.
		''' Returns null if none could be discerned from the responses
		''' (the result was not valid HTTP). </summary>
		''' <exception cref="IOException"> if an error occurred connecting to the server. </exception>
		''' <returns> the HTTP response message, or {@code null} </returns>
		Public Overridable Property responseMessage As String
			Get
				responseCode
				Return responseMessage
			End Get
		End Property

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Overrides Function getHeaderFieldDate(ByVal name As String, ByVal [Default] As Long) As Long
			Dim dateString As String = getHeaderField(name)
			Try
				If dateString.IndexOf("GMT") = -1 Then dateString = dateString & " GMT"
				Return DateTime.Parse(dateString)
			Catch e As Exception
			End Try
			Return [Default]
		End Function


		''' <summary>
		''' Indicates that other requests to the server
		''' are unlikely in the near future. Calling disconnect()
		''' should not imply that this HttpURLConnection
		''' instance can be reused for other requests.
		''' </summary>
		Public MustOverride Sub disconnect()

		''' <summary>
		''' Indicates if the connection is going through a proxy. </summary>
		''' <returns> a boolean indicating if the connection is
		''' using a proxy. </returns>
		Public MustOverride Function usingProxy() As Boolean

		''' <summary>
		''' Returns a <seealso cref="SocketPermission"/> object representing the
		''' permission necessary to connect to the destination host and port.
		''' </summary>
		''' <exception cref="IOException"> if an error occurs while computing
		'''            the permission.
		''' </exception>
		''' <returns> a {@code SocketPermission} object representing the
		'''         permission necessary to connect to the destination
		'''         host and port. </returns>
		Public  Overrides ReadOnly Property  permission As java.security.Permission
			Get
				Dim port As Integer = url.port
				port = If(port < 0, 80, port)
				Dim host As String = url.host & ":" & port
				Dim permission_Renamed As java.security.Permission = New SocketPermission(host, "connect")
				Return permission_Renamed
			End Get
		End Property

	   ''' <summary>
	   ''' Returns the error stream if the connection failed
	   ''' but the server sent useful data nonetheless. The
	   ''' typical example is when an HTTP server responds
	   ''' with a 404, which will cause a FileNotFoundException
	   ''' to be thrown in connect, but the server sent an HTML
	   ''' help page with suggestions as to what to do.
	   ''' 
	   ''' <p>This method will not cause a connection to be initiated.  If
	   ''' the connection was not connected, or if the server did not have
	   ''' an error while connecting or if the server had an error but
	   ''' no error data was sent, this method will return null. This is
	   ''' the default.
	   ''' </summary>
	   ''' <returns> an error stream if any, null if there have been no
	   ''' errors, the connection is not connected or the server sent no
	   ''' useful data. </returns>
		Public Overridable Property errorStream As java.io.InputStream
			Get
				Return Nothing
			End Get
		End Property

		''' <summary>
		''' The response codes for HTTP, as of version 1.1.
		''' </summary>

		' REMIND: do we want all these??
		' Others not here that we do want??

		' 2XX: generally "OK" 

		''' <summary>
		''' HTTP Status-Code 200: OK.
		''' </summary>
		Public Const HTTP_OK As Integer = 200

		''' <summary>
		''' HTTP Status-Code 201: Created.
		''' </summary>
		Public Const HTTP_CREATED As Integer = 201

		''' <summary>
		''' HTTP Status-Code 202: Accepted.
		''' </summary>
		Public Const HTTP_ACCEPTED As Integer = 202

		''' <summary>
		''' HTTP Status-Code 203: Non-Authoritative Information.
		''' </summary>
		Public Const HTTP_NOT_AUTHORITATIVE As Integer = 203

		''' <summary>
		''' HTTP Status-Code 204: No Content.
		''' </summary>
		Public Const HTTP_NO_CONTENT As Integer = 204

		''' <summary>
		''' HTTP Status-Code 205: Reset Content.
		''' </summary>
		Public Const HTTP_RESET As Integer = 205

		''' <summary>
		''' HTTP Status-Code 206: Partial Content.
		''' </summary>
		Public Const HTTP_PARTIAL As Integer = 206

		' 3XX: relocation/redirect 

		''' <summary>
		''' HTTP Status-Code 300: Multiple Choices.
		''' </summary>
		Public Const HTTP_MULT_CHOICE As Integer = 300

		''' <summary>
		''' HTTP Status-Code 301: Moved Permanently.
		''' </summary>
		Public Const HTTP_MOVED_PERM As Integer = 301

		''' <summary>
		''' HTTP Status-Code 302: Temporary Redirect.
		''' </summary>
		Public Const HTTP_MOVED_TEMP As Integer = 302

		''' <summary>
		''' HTTP Status-Code 303: See Other.
		''' </summary>
		Public Const HTTP_SEE_OTHER As Integer = 303

		''' <summary>
		''' HTTP Status-Code 304: Not Modified.
		''' </summary>
		Public Const HTTP_NOT_MODIFIED As Integer = 304

		''' <summary>
		''' HTTP Status-Code 305: Use Proxy.
		''' </summary>
		Public Const HTTP_USE_PROXY As Integer = 305

		' 4XX: client error 

		''' <summary>
		''' HTTP Status-Code 400: Bad Request.
		''' </summary>
		Public Const HTTP_BAD_REQUEST As Integer = 400

		''' <summary>
		''' HTTP Status-Code 401: Unauthorized.
		''' </summary>
		Public Const HTTP_UNAUTHORIZED As Integer = 401

		''' <summary>
		''' HTTP Status-Code 402: Payment Required.
		''' </summary>
		Public Const HTTP_PAYMENT_REQUIRED As Integer = 402

		''' <summary>
		''' HTTP Status-Code 403: Forbidden.
		''' </summary>
		Public Const HTTP_FORBIDDEN As Integer = 403

		''' <summary>
		''' HTTP Status-Code 404: Not Found.
		''' </summary>
		Public Const HTTP_NOT_FOUND As Integer = 404

		''' <summary>
		''' HTTP Status-Code 405: Method Not Allowed.
		''' </summary>
		Public Const HTTP_BAD_METHOD As Integer = 405

		''' <summary>
		''' HTTP Status-Code 406: Not Acceptable.
		''' </summary>
		Public Const HTTP_NOT_ACCEPTABLE As Integer = 406

		''' <summary>
		''' HTTP Status-Code 407: Proxy Authentication Required.
		''' </summary>
		Public Const HTTP_PROXY_AUTH As Integer = 407

		''' <summary>
		''' HTTP Status-Code 408: Request Time-Out.
		''' </summary>
		Public Const HTTP_CLIENT_TIMEOUT As Integer = 408

		''' <summary>
		''' HTTP Status-Code 409: Conflict.
		''' </summary>
		Public Const HTTP_CONFLICT As Integer = 409

		''' <summary>
		''' HTTP Status-Code 410: Gone.
		''' </summary>
		Public Const HTTP_GONE As Integer = 410

		''' <summary>
		''' HTTP Status-Code 411: Length Required.
		''' </summary>
		Public Const HTTP_LENGTH_REQUIRED As Integer = 411

		''' <summary>
		''' HTTP Status-Code 412: Precondition Failed.
		''' </summary>
		Public Const HTTP_PRECON_FAILED As Integer = 412

		''' <summary>
		''' HTTP Status-Code 413: Request Entity Too Large.
		''' </summary>
		Public Const HTTP_ENTITY_TOO_LARGE As Integer = 413

		''' <summary>
		''' HTTP Status-Code 414: Request-URI Too Large.
		''' </summary>
		Public Const HTTP_REQ_TOO_LONG As Integer = 414

		''' <summary>
		''' HTTP Status-Code 415: Unsupported Media Type.
		''' </summary>
		Public Const HTTP_UNSUPPORTED_TYPE As Integer = 415

		' 5XX: server error 

		''' <summary>
		''' HTTP Status-Code 500: Internal Server Error. </summary>
		''' @deprecated   it is misplaced and shouldn't have existed. 
		<Obsolete("  it is misplaced and shouldn't have existed.")> _
		Public Const HTTP_SERVER_ERROR As Integer = 500

		''' <summary>
		''' HTTP Status-Code 500: Internal Server Error.
		''' </summary>
		Public Const HTTP_INTERNAL_ERROR As Integer = 500

		''' <summary>
		''' HTTP Status-Code 501: Not Implemented.
		''' </summary>
		Public Const HTTP_NOT_IMPLEMENTED As Integer = 501

		''' <summary>
		''' HTTP Status-Code 502: Bad Gateway.
		''' </summary>
		Public Const HTTP_BAD_GATEWAY As Integer = 502

		''' <summary>
		''' HTTP Status-Code 503: Service Unavailable.
		''' </summary>
		Public Const HTTP_UNAVAILABLE As Integer = 503

		''' <summary>
		''' HTTP Status-Code 504: Gateway Timeout.
		''' </summary>
		Public Const HTTP_GATEWAY_TIMEOUT As Integer = 504

		''' <summary>
		''' HTTP Status-Code 505: HTTP Version Not Supported.
		''' </summary>
		Public Const HTTP_VERSION As Integer = 505

	End Class

End Namespace