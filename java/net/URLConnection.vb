Imports Microsoft.VisualBasic
Imports System
Imports System.Runtime.CompilerServices
Imports System.Collections.Generic

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
	''' The abstract class {@code URLConnection} is the superclass
	''' of all classes that represent a communications link between the
	''' application and a URL. Instances of this class can be used both to
	''' read from and to write to the resource referenced by the URL. In
	''' general, creating a connection to a URL is a multistep process:
	''' 
	''' <center><table border=2 summary="Describes the process of creating a connection to a URL: openConnection() and connect() over time.">
	''' <tr><th>{@code openConnection()}</th>
	'''     <th>{@code connect()}</th></tr>
	''' <tr><td>Manipulate parameters that affect the connection to the remote
	'''         resource.</td>
	'''     <td>Interact with the resource; query header fields and
	'''         contents.</td></tr>
	''' </table>
	''' ----------------------------&gt;
	''' <br>time</center>
	''' 
	''' <ol>
	''' <li>The connection object is created by invoking the
	'''     {@code openConnection} method on a URL.
	''' <li>The setup parameters and general request properties are manipulated.
	''' <li>The actual connection to the remote object is made, using the
	'''    {@code connect} method.
	''' <li>The remote object becomes available. The header fields and the contents
	'''     of the remote object can be accessed.
	''' </ol>
	''' <p>
	''' The setup parameters are modified using the following methods:
	''' <ul>
	'''   <li>{@code setAllowUserInteraction}
	'''   <li>{@code setDoInput}
	'''   <li>{@code setDoOutput}
	'''   <li>{@code setIfModifiedSince}
	'''   <li>{@code setUseCaches}
	''' </ul>
	''' <p>
	''' and the general request properties are modified using the method:
	''' <ul>
	'''   <li>{@code setRequestProperty}
	''' </ul>
	''' <p>
	''' Default values for the {@code AllowUserInteraction} and
	''' {@code UseCaches} parameters can be set using the methods
	''' {@code setDefaultAllowUserInteraction} and
	''' {@code setDefaultUseCaches}.
	''' <p>
	''' Each of the above {@code set} methods has a corresponding
	''' {@code get} method to retrieve the value of the parameter or
	''' general request property. The specific parameters and general
	''' request properties that are applicable are protocol specific.
	''' <p>
	''' The following methods are used to access the header fields and
	''' the contents after the connection is made to the remote object:
	''' <ul>
	'''   <li>{@code getContent}
	'''   <li>{@code getHeaderField}
	'''   <li>{@code getInputStream}
	'''   <li>{@code getOutputStream}
	''' </ul>
	''' <p>
	''' Certain header fields are accessed frequently. The methods:
	''' <ul>
	'''   <li>{@code getContentEncoding}
	'''   <li>{@code getContentLength}
	'''   <li>{@code getContentType}
	'''   <li>{@code getDate}
	'''   <li>{@code getExpiration}
	'''   <li>{@code getLastModifed}
	''' </ul>
	''' <p>
	''' provide convenient access to these fields. The
	''' {@code getContentType} method is used by the
	''' {@code getContent} method to determine the type of the remote
	''' object; subclasses may find it convenient to override the
	''' {@code getContentType} method.
	''' <p>
	''' In the common case, all of the pre-connection parameters and
	''' general request properties can be ignored: the pre-connection
	''' parameters and request properties default to sensible values. For
	''' most clients of this interface, there are only two interesting
	''' methods: {@code getInputStream} and {@code getContent},
	''' which are mirrored in the {@code URL} class by convenience methods.
	''' <p>
	''' More information on the request properties and header fields of
	''' an {@code http} connection can be found at:
	''' <blockquote><pre>
	''' <a href="http://www.ietf.org/rfc/rfc2616.txt">http://www.ietf.org/rfc/rfc2616.txt</a>
	''' </pre></blockquote>
	''' 
	''' Invoking the {@code close()} methods on the {@code InputStream} or {@code OutputStream} of an
	''' {@code URLConnection} after a request may free network resources associated with this
	''' instance, unless particular protocol specifications specify different behaviours
	''' for it.
	''' 
	''' @author  James Gosling </summary>
	''' <seealso cref=     java.net.URL#openConnection() </seealso>
	''' <seealso cref=     java.net.URLConnection#connect() </seealso>
	''' <seealso cref=     java.net.URLConnection#getContent() </seealso>
	''' <seealso cref=     java.net.URLConnection#getContentEncoding() </seealso>
	''' <seealso cref=     java.net.URLConnection#getContentLength() </seealso>
	''' <seealso cref=     java.net.URLConnection#getContentType() </seealso>
	''' <seealso cref=     java.net.URLConnection#getDate() </seealso>
	''' <seealso cref=     java.net.URLConnection#getExpiration() </seealso>
	''' <seealso cref=     java.net.URLConnection#getHeaderField(int) </seealso>
	''' <seealso cref=     java.net.URLConnection#getHeaderField(java.lang.String) </seealso>
	''' <seealso cref=     java.net.URLConnection#getInputStream() </seealso>
	''' <seealso cref=     java.net.URLConnection#getLastModified() </seealso>
	''' <seealso cref=     java.net.URLConnection#getOutputStream() </seealso>
	''' <seealso cref=     java.net.URLConnection#setAllowUserInteraction(boolean) </seealso>
	''' <seealso cref=     java.net.URLConnection#setDefaultUseCaches(boolean) </seealso>
	''' <seealso cref=     java.net.URLConnection#setDoInput(boolean) </seealso>
	''' <seealso cref=     java.net.URLConnection#setDoOutput(boolean) </seealso>
	''' <seealso cref=     java.net.URLConnection#setIfModifiedSince(long) </seealso>
	''' <seealso cref=     java.net.URLConnection#setRequestProperty(java.lang.String, java.lang.String) </seealso>
	''' <seealso cref=     java.net.URLConnection#setUseCaches(boolean)
	''' @since   JDK1.0 </seealso>
	Public MustInherit Class URLConnection

	   ''' <summary>
	   ''' The URL represents the remote object on the World Wide Web to
	   ''' which this connection is opened.
	   ''' <p>
	   ''' The value of this field can be accessed by the
	   ''' {@code getURL} method.
	   ''' <p>
	   ''' The default value of this variable is the value of the URL
	   ''' argument in the {@code URLConnection} constructor.
	   ''' </summary>
	   ''' <seealso cref=     java.net.URLConnection#getURL() </seealso>
	   ''' <seealso cref=     java.net.URLConnection#url </seealso>
		Protected Friend url As URL

	   ''' <summary>
	   ''' This variable is set by the {@code setDoInput} method. Its
	   ''' value is returned by the {@code getDoInput} method.
	   ''' <p>
	   ''' A URL connection can be used for input and/or output. Setting the
	   ''' {@code doInput} flag to {@code true} indicates that
	   ''' the application intends to read data from the URL connection.
	   ''' <p>
	   ''' The default value of this field is {@code true}.
	   ''' </summary>
	   ''' <seealso cref=     java.net.URLConnection#getDoInput() </seealso>
	   ''' <seealso cref=     java.net.URLConnection#setDoInput(boolean) </seealso>
		Protected Friend doInput As Boolean = True

	   ''' <summary>
	   ''' This variable is set by the {@code setDoOutput} method. Its
	   ''' value is returned by the {@code getDoOutput} method.
	   ''' <p>
	   ''' A URL connection can be used for input and/or output. Setting the
	   ''' {@code doOutput} flag to {@code true} indicates
	   ''' that the application intends to write data to the URL connection.
	   ''' <p>
	   ''' The default value of this field is {@code false}.
	   ''' </summary>
	   ''' <seealso cref=     java.net.URLConnection#getDoOutput() </seealso>
	   ''' <seealso cref=     java.net.URLConnection#setDoOutput(boolean) </seealso>
		Protected Friend doOutput As Boolean = False

		Private Shared defaultAllowUserInteraction As Boolean = False

	   ''' <summary>
	   ''' If {@code true}, this {@code URL} is being examined in
	   ''' a context in which it makes sense to allow user interactions such
	   ''' as popping up an authentication dialog. If {@code false},
	   ''' then no user interaction is allowed.
	   ''' <p>
	   ''' The value of this field can be set by the
	   ''' {@code setAllowUserInteraction} method.
	   ''' Its value is returned by the
	   ''' {@code getAllowUserInteraction} method.
	   ''' Its default value is the value of the argument in the last invocation
	   ''' of the {@code setDefaultAllowUserInteraction} method.
	   ''' </summary>
	   ''' <seealso cref=     java.net.URLConnection#getAllowUserInteraction() </seealso>
	   ''' <seealso cref=     java.net.URLConnection#setAllowUserInteraction(boolean) </seealso>
	   ''' <seealso cref=     java.net.URLConnection#setDefaultAllowUserInteraction(boolean) </seealso>
		Protected Friend allowUserInteraction As Boolean = defaultAllowUserInteraction

		Private Shared defaultUseCaches As Boolean = True

	   ''' <summary>
	   ''' If {@code true}, the protocol is allowed to use caching
	   ''' whenever it can. If {@code false}, the protocol must always
	   ''' try to get a fresh copy of the object.
	   ''' <p>
	   ''' This field is set by the {@code setUseCaches} method. Its
	   ''' value is returned by the {@code getUseCaches} method.
	   ''' <p>
	   ''' Its default value is the value given in the last invocation of the
	   ''' {@code setDefaultUseCaches} method.
	   ''' </summary>
	   ''' <seealso cref=     java.net.URLConnection#setUseCaches(boolean) </seealso>
	   ''' <seealso cref=     java.net.URLConnection#getUseCaches() </seealso>
	   ''' <seealso cref=     java.net.URLConnection#setDefaultUseCaches(boolean) </seealso>
		Protected Friend useCaches As Boolean = defaultUseCaches

	   ''' <summary>
	   ''' Some protocols support skipping the fetching of the object unless
	   ''' the object has been modified more recently than a certain time.
	   ''' <p>
	   ''' A nonzero value gives a time as the number of milliseconds since
	   ''' January 1, 1970, GMT. The object is fetched only if it has been
	   ''' modified more recently than that time.
	   ''' <p>
	   ''' This variable is set by the {@code setIfModifiedSince}
	   ''' method. Its value is returned by the
	   ''' {@code getIfModifiedSince} method.
	   ''' <p>
	   ''' The default value of this field is {@code 0}, indicating
	   ''' that the fetching must always occur.
	   ''' </summary>
	   ''' <seealso cref=     java.net.URLConnection#getIfModifiedSince() </seealso>
	   ''' <seealso cref=     java.net.URLConnection#setIfModifiedSince(long) </seealso>
		Protected Friend ifModifiedSince As Long = 0

	   ''' <summary>
	   ''' If {@code false}, this connection object has not created a
	   ''' communications link to the specified URL. If {@code true},
	   ''' the communications link has been established.
	   ''' </summary>
		Protected Friend connected As Boolean = False

		''' <summary>
		''' @since 1.5
		''' </summary>
		Private connectTimeout As Integer
		Private readTimeout As Integer

		''' <summary>
		''' @since 1.6
		''' </summary>
		Private requests As sun.net.www.MessageHeader

	   ''' <summary>
	   ''' @since   JDK1.1
	   ''' </summary>
		Private Shared fileNameMap As FileNameMap

		''' <summary>
		''' @since 1.2.2
		''' </summary>
		Private Shared fileNameMapLoaded As Boolean = False

		''' <summary>
		''' Loads filename map (a mimetable) from a data file. It will
		''' first try to load the user-specific table, defined
		''' by &quot;content.types.user.table&quot; property. If that fails,
		''' it tries to load the default built-in table.
		''' </summary>
		''' <returns> the FileNameMap
		''' @since 1.2 </returns>
		''' <seealso cref= #setFileNameMap(java.net.FileNameMap) </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Property Shared fileNameMap As FileNameMap
			Get
				If (fileNameMap Is Nothing) AndAlso (Not fileNameMapLoaded) Then
					fileNameMap = sun.net.www.MimeTable.loadTable()
					fileNameMapLoaded = True
				End If
    
				Return New FileNameMapAnonymousInnerClassHelper
			End Get
		End Property

		Private Class FileNameMapAnonymousInnerClassHelper
			Implements FileNameMap

			Private map As FileNameMap = fileNameMap
			Public Overridable Function getContentTypeFor(ByVal fileName As String) As String Implements FileNameMap.getContentTypeFor
				Return map.getContentTypeFor(fileName)
			End Function
		End Class

		''' <summary>
		''' Sets the FileNameMap.
		''' <p>
		''' If there is a security manager, this method first calls
		''' the security manager's {@code checkSetFactory} method
		''' to ensure the operation is allowed.
		''' This could result in a SecurityException.
		''' </summary>
		''' <param name="map"> the FileNameMap to be set </param>
		''' <exception cref="SecurityException">  if a security manager exists and its
		'''             {@code checkSetFactory} method doesn't allow the operation. </exception>
		''' <seealso cref=        SecurityManager#checkSetFactory </seealso>
		''' <seealso cref= #getFileNameMap()
		''' @since 1.2 </seealso>
		Public Shared Property fileNameMap As FileNameMap
			Set(ByVal map As FileNameMap)
				Dim sm As SecurityManager = System.securityManager
				If sm IsNot Nothing Then sm.checkSetFactory()
				fileNameMap = map
			End Set
		End Property

		''' <summary>
		''' Opens a communications link to the resource referenced by this
		''' URL, if such a connection has not already been established.
		''' <p>
		''' If the {@code connect} method is called when the connection
		''' has already been opened (indicated by the {@code connected}
		''' field having the value {@code true}), the call is ignored.
		''' <p>
		''' URLConnection objects go through two phases: first they are
		''' created, then they are connected.  After being created, and
		''' before being connected, various options can be specified
		''' (e.g., doInput and UseCaches).  After connecting, it is an
		''' error to try to set them.  Operations that depend on being
		''' connected, like getContentLength, will implicitly perform the
		''' connection, if necessary.
		''' </summary>
		''' <exception cref="SocketTimeoutException"> if the timeout expires before
		'''               the connection can be established </exception>
		''' <exception cref="IOException">  if an I/O error occurs while opening the
		'''               connection. </exception>
		''' <seealso cref= java.net.URLConnection#connected </seealso>
		''' <seealso cref= #getConnectTimeout() </seealso>
		''' <seealso cref= #setConnectTimeout(int) </seealso>
		Public MustOverride Sub connect()

		''' <summary>
		''' Sets a specified timeout value, in milliseconds, to be used
		''' when opening a communications link to the resource referenced
		''' by this URLConnection.  If the timeout expires before the
		''' connection can be established, a
		''' java.net.SocketTimeoutException is raised. A timeout of zero is
		''' interpreted as an infinite timeout.
		''' 
		''' <p> Some non-standard implementation of this method may ignore
		''' the specified timeout. To see the connect timeout set, please
		''' call getConnectTimeout().
		''' </summary>
		''' <param name="timeout"> an {@code int} that specifies the connect
		'''               timeout value in milliseconds </param>
		''' <exception cref="IllegalArgumentException"> if the timeout parameter is negative
		''' </exception>
		''' <seealso cref= #getConnectTimeout() </seealso>
		''' <seealso cref= #connect()
		''' @since 1.5 </seealso>
		Public Overridable Property connectTimeout As Integer
			Set(ByVal timeout As Integer)
				If timeout < 0 Then Throw New IllegalArgumentException("timeout can not be negative")
				connectTimeout = timeout
			End Set
			Get
				Return connectTimeout
			End Get
		End Property


		''' <summary>
		''' Sets the read timeout to a specified timeout, in
		''' milliseconds. A non-zero value specifies the timeout when
		''' reading from Input stream when a connection is established to a
		''' resource. If the timeout expires before there is data available
		''' for read, a java.net.SocketTimeoutException is raised. A
		''' timeout of zero is interpreted as an infinite timeout.
		''' 
		''' <p> Some non-standard implementation of this method ignores the
		''' specified timeout. To see the read timeout set, please call
		''' getReadTimeout().
		''' </summary>
		''' <param name="timeout"> an {@code int} that specifies the timeout
		''' value to be used in milliseconds </param>
		''' <exception cref="IllegalArgumentException"> if the timeout parameter is negative
		''' </exception>
		''' <seealso cref= #getReadTimeout() </seealso>
		''' <seealso cref= InputStream#read()
		''' @since 1.5 </seealso>
		Public Overridable Property readTimeout As Integer
			Set(ByVal timeout As Integer)
				If timeout < 0 Then Throw New IllegalArgumentException("timeout can not be negative")
				readTimeout = timeout
			End Set
			Get
				Return readTimeout
			End Get
		End Property


		''' <summary>
		''' Constructs a URL connection to the specified URL. A connection to
		''' the object referenced by the URL is not created.
		''' </summary>
		''' <param name="url">   the specified URL. </param>
		Protected Friend Sub New(ByVal url As URL)
			Me.url = url
		End Sub

		''' <summary>
		''' Returns the value of this {@code URLConnection}'s {@code URL}
		''' field.
		''' </summary>
		''' <returns>  the value of this {@code URLConnection}'s {@code URL}
		'''          field. </returns>
		''' <seealso cref=     java.net.URLConnection#url </seealso>
		Public Overridable Property uRL As URL
			Get
				Return url
			End Get
		End Property

		''' <summary>
		''' Returns the value of the {@code content-length} header field.
		''' <P>
		''' <B>Note</B>: <seealso cref="#getContentLengthLong() getContentLengthLong()"/>
		''' should be preferred over this method, since it returns a {@code long}
		''' instead and is therefore more portable.</P>
		''' </summary>
		''' <returns>  the content length of the resource that this connection's URL
		'''          references, {@code -1} if the content length is not known,
		'''          or if the content length is greater than Integer.MAX_VALUE. </returns>
		Public Overridable Property contentLength As Integer
			Get
				Dim l As Long = contentLengthLong
				If l > Integer.MaxValue Then Return -1
				Return CInt(l)
			End Get
		End Property

		''' <summary>
		''' Returns the value of the {@code content-length} header field as a
		''' long.
		''' </summary>
		''' <returns>  the content length of the resource that this connection's URL
		'''          references, or {@code -1} if the content length is
		'''          not known.
		''' @since 7.0 </returns>
		Public Overridable Property contentLengthLong As Long
			Get
				Return getHeaderFieldLong("content-length", -1)
			End Get
		End Property

		''' <summary>
		''' Returns the value of the {@code content-type} header field.
		''' </summary>
		''' <returns>  the content type of the resource that the URL references,
		'''          or {@code null} if not known. </returns>
		''' <seealso cref=     java.net.URLConnection#getHeaderField(java.lang.String) </seealso>
		Public Overridable Property contentType As String
			Get
				Return getHeaderField("content-type")
			End Get
		End Property

		''' <summary>
		''' Returns the value of the {@code content-encoding} header field.
		''' </summary>
		''' <returns>  the content encoding of the resource that the URL references,
		'''          or {@code null} if not known. </returns>
		''' <seealso cref=     java.net.URLConnection#getHeaderField(java.lang.String) </seealso>
		Public Overridable Property contentEncoding As String
			Get
				Return getHeaderField("content-encoding")
			End Get
		End Property

		''' <summary>
		''' Returns the value of the {@code expires} header field.
		''' </summary>
		''' <returns>  the expiration date of the resource that this URL references,
		'''          or 0 if not known. The value is the number of milliseconds since
		'''          January 1, 1970 GMT. </returns>
		''' <seealso cref=     java.net.URLConnection#getHeaderField(java.lang.String) </seealso>
		Public Overridable Property expiration As Long
			Get
				Return getHeaderFieldDate("expires", 0)
			End Get
		End Property

		''' <summary>
		''' Returns the value of the {@code date} header field.
		''' </summary>
		''' <returns>  the sending date of the resource that the URL references,
		'''          or {@code 0} if not known. The value returned is the
		'''          number of milliseconds since January 1, 1970 GMT. </returns>
		''' <seealso cref=     java.net.URLConnection#getHeaderField(java.lang.String) </seealso>
		Public Overridable Property [date] As Long
			Get
				Return getHeaderFieldDate("date", 0)
			End Get
		End Property

		''' <summary>
		''' Returns the value of the {@code last-modified} header field.
		''' The result is the number of milliseconds since January 1, 1970 GMT.
		''' </summary>
		''' <returns>  the date the resource referenced by this
		'''          {@code URLConnection} was last modified, or 0 if not known. </returns>
		''' <seealso cref=     java.net.URLConnection#getHeaderField(java.lang.String) </seealso>
		Public Overridable Property lastModified As Long
			Get
				Return getHeaderFieldDate("last-modified", 0)
			End Get
		End Property

		''' <summary>
		''' Returns the value of the named header field.
		''' <p>
		''' If called on a connection that sets the same header multiple times
		''' with possibly different values, only the last value is returned.
		''' 
		''' </summary>
		''' <param name="name">   the name of a header field. </param>
		''' <returns>  the value of the named header field, or {@code null}
		'''          if there is no such field in the header. </returns>
		Public Overridable Function getHeaderField(ByVal name As String) As String
			Return Nothing
		End Function

		''' <summary>
		''' Returns an unmodifiable Map of the header fields.
		''' The Map keys are Strings that represent the
		''' response-header field names. Each Map value is an
		''' unmodifiable List of Strings that represents
		''' the corresponding field values.
		''' </summary>
		''' <returns> a Map of header fields
		''' @since 1.4 </returns>
		Public Overridable Property headerFields As IDictionary(Of String, IList(Of String))
			Get
				Return java.util.Collections.emptyMap()
			End Get
		End Property

		''' <summary>
		''' Returns the value of the named field parsed as a number.
		''' <p>
		''' This form of {@code getHeaderField} exists because some
		''' connection types (e.g., {@code http-ng}) have pre-parsed
		''' headers. Classes for that connection type can override this method
		''' and short-circuit the parsing.
		''' </summary>
		''' <param name="name">      the name of the header field. </param>
		''' <param name="Default">   the default value. </param>
		''' <returns>  the value of the named field, parsed as an integer. The
		'''          {@code Default} value is returned if the field is
		'''          missing or malformed. </returns>
		Public Overridable Function getHeaderFieldInt(ByVal name As String, ByVal [Default] As Integer) As Integer
			Dim value As String = getHeaderField(name)
			Try
				Return Convert.ToInt32(value)
			Catch e As Exception
			End Try
			Return [Default]
		End Function

		''' <summary>
		''' Returns the value of the named field parsed as a number.
		''' <p>
		''' This form of {@code getHeaderField} exists because some
		''' connection types (e.g., {@code http-ng}) have pre-parsed
		''' headers. Classes for that connection type can override this method
		''' and short-circuit the parsing.
		''' </summary>
		''' <param name="name">      the name of the header field. </param>
		''' <param name="Default">   the default value. </param>
		''' <returns>  the value of the named field, parsed as a long. The
		'''          {@code Default} value is returned if the field is
		'''          missing or malformed.
		''' @since 7.0 </returns>
		Public Overridable Function getHeaderFieldLong(ByVal name As String, ByVal [Default] As Long) As Long
			Dim value As String = getHeaderField(name)
			Try
				Return Convert.ToInt64(value)
			Catch e As Exception
			End Try
			Return [Default]
		End Function

		''' <summary>
		''' Returns the value of the named field parsed as date.
		''' The result is the number of milliseconds since January 1, 1970 GMT
		''' represented by the named field.
		''' <p>
		''' This form of {@code getHeaderField} exists because some
		''' connection types (e.g., {@code http-ng}) have pre-parsed
		''' headers. Classes for that connection type can override this method
		''' and short-circuit the parsing.
		''' </summary>
		''' <param name="name">     the name of the header field. </param>
		''' <param name="Default">   a default value. </param>
		''' <returns>  the value of the field, parsed as a date. The value of the
		'''          {@code Default} argument is returned if the field is
		'''          missing or malformed. </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Overridable Function getHeaderFieldDate(ByVal name As String, ByVal [Default] As Long) As Long
			Dim value As String = getHeaderField(name)
			Try
				Return DateTime.Parse(value)
			Catch e As Exception
			End Try
			Return [Default]
		End Function

		''' <summary>
		''' Returns the key for the {@code n}<sup>th</sup> header field.
		''' It returns {@code null} if there are fewer than {@code n+1} fields.
		''' </summary>
		''' <param name="n">   an index, where {@code n>=0} </param>
		''' <returns>  the key for the {@code n}<sup>th</sup> header field,
		'''          or {@code null} if there are fewer than {@code n+1}
		'''          fields. </returns>
		Public Overridable Function getHeaderFieldKey(ByVal n As Integer) As String
			Return Nothing
		End Function

		''' <summary>
		''' Returns the value for the {@code n}<sup>th</sup> header field.
		''' It returns {@code null} if there are fewer than
		''' {@code n+1}fields.
		''' <p>
		''' This method can be used in conjunction with the
		''' <seealso cref="#getHeaderFieldKey(int) getHeaderFieldKey"/> method to iterate through all
		''' the headers in the message.
		''' </summary>
		''' <param name="n">   an index, where {@code n>=0} </param>
		''' <returns>  the value of the {@code n}<sup>th</sup> header field
		'''          or {@code null} if there are fewer than {@code n+1} fields </returns>
		''' <seealso cref=     java.net.URLConnection#getHeaderFieldKey(int) </seealso>
		Public Overridable Function getHeaderField(ByVal n As Integer) As String
			Return Nothing
		End Function

		''' <summary>
		''' Retrieves the contents of this URL connection.
		''' <p>
		''' This method first determines the content type of the object by
		''' calling the {@code getContentType} method. If this is
		''' the first time that the application has seen that specific content
		''' type, a content handler for that content type is created:
		''' <ol>
		''' <li>If the application has set up a content handler factory instance
		'''     using the {@code setContentHandlerFactory} method, the
		'''     {@code createContentHandler} method of that instance is called
		'''     with the content type as an argument; the result is a content
		'''     handler for that content type.
		''' <li>If no content handler factory has yet been set up, or if the
		'''     factory's {@code createContentHandler} method returns
		'''     {@code null}, then the application loads the class named:
		'''     <blockquote><pre>
		'''         sun.net.www.content.&lt;<i>contentType</i>&gt;
		'''     </pre></blockquote>
		'''     where &lt;<i>contentType</i>&gt; is formed by taking the
		'''     content-type string, replacing all slash characters with a
		'''     {@code period} ('.'), and all other non-alphanumeric characters
		'''     with the underscore character '{@code _}'. The alphanumeric
		'''     characters are specifically the 26 uppercase ASCII letters
		'''     '{@code A}' through '{@code Z}', the 26 lowercase ASCII
		'''     letters '{@code a}' through '{@code z}', and the 10 ASCII
		'''     digits '{@code 0}' through '{@code 9}'. If the specified
		'''     class does not exist, or is not a subclass of
		'''     {@code ContentHandler}, then an
		'''     {@code UnknownServiceException} is thrown.
		''' </ol>
		''' </summary>
		''' <returns>     the object fetched. The {@code instanceof} operator
		'''               should be used to determine the specific kind of object
		'''               returned. </returns>
		''' <exception cref="IOException">              if an I/O error occurs while
		'''               getting the content. </exception>
		''' <exception cref="UnknownServiceException">  if the protocol does not support
		'''               the content type. </exception>
		''' <seealso cref=        java.net.ContentHandlerFactory#createContentHandler(java.lang.String) </seealso>
		''' <seealso cref=        java.net.URLConnection#getContentType() </seealso>
		''' <seealso cref=        java.net.URLConnection#setContentHandlerFactory(java.net.ContentHandlerFactory) </seealso>
		Public Overridable Property content As Object
			Get
				' Must call getInputStream before GetHeaderField gets called
				' so that FileNotFoundException has a chance to be thrown up
				' from here without being caught.
				inputStream
				Return contentHandler.getContent(Me)
			End Get
		End Property

		''' <summary>
		''' Retrieves the contents of this URL connection.
		''' </summary>
		''' <param name="classes"> the {@code Class} array
		''' indicating the requested types </param>
		''' <returns>     the object fetched that is the first match of the type
		'''               specified in the classes array. null if none of
		'''               the requested types are supported.
		'''               The {@code instanceof} operator should be used to
		'''               determine the specific kind of object returned. </returns>
		''' <exception cref="IOException">              if an I/O error occurs while
		'''               getting the content. </exception>
		''' <exception cref="UnknownServiceException">  if the protocol does not support
		'''               the content type. </exception>
		''' <seealso cref=        java.net.URLConnection#getContent() </seealso>
		''' <seealso cref=        java.net.ContentHandlerFactory#createContentHandler(java.lang.String) </seealso>
		''' <seealso cref=        java.net.URLConnection#getContent(java.lang.Class[]) </seealso>
		''' <seealso cref=        java.net.URLConnection#setContentHandlerFactory(java.net.ContentHandlerFactory)
		''' @since 1.3 </seealso>
		Public Overridable Function getContent(ByVal classes As  [Class]()) As Object
			' Must call getInputStream before GetHeaderField gets called
			' so that FileNotFoundException has a chance to be thrown up
			' from here without being caught.
			inputStream
			Return contentHandler.getContent(Me, classes)
		End Function

		''' <summary>
		''' Returns a permission object representing the permission
		''' necessary to make the connection represented by this
		''' object. This method returns null if no permission is
		''' required to make the connection. By default, this method
		''' returns {@code java.security.AllPermission}. Subclasses
		''' should override this method and return the permission
		''' that best represents the permission required to make a
		''' a connection to the URL. For example, a {@code URLConnection}
		''' representing a {@code file:} URL would return a
		''' {@code java.io.FilePermission} object.
		''' 
		''' <p>The permission returned may dependent upon the state of the
		''' connection. For example, the permission before connecting may be
		''' different from that after connecting. For example, an HTTP
		''' sever, say foo.com, may redirect the connection to a different
		''' host, say bar.com. Before connecting the permission returned by
		''' the connection will represent the permission needed to connect
		''' to foo.com, while the permission returned after connecting will
		''' be to bar.com.
		''' 
		''' <p>Permissions are generally used for two purposes: to protect
		''' caches of objects obtained through URLConnections, and to check
		''' the right of a recipient to learn about a particular URL. In
		''' the first case, the permission should be obtained
		''' <em>after</em> the object has been obtained. For example, in an
		''' HTTP connection, this will represent the permission to connect
		''' to the host from which the data was ultimately fetched. In the
		''' second case, the permission should be obtained and tested
		''' <em>before</em> connecting.
		''' </summary>
		''' <returns> the permission object representing the permission
		''' necessary to make the connection represented by this
		''' URLConnection.
		''' </returns>
		''' <exception cref="IOException"> if the computation of the permission
		''' requires network or file I/O and an exception occurs while
		''' computing it. </exception>
		Public Overridable Property permission As java.security.Permission
			Get
				Return sun.security.util.SecurityConstants.ALL_PERMISSION
			End Get
		End Property

		''' <summary>
		''' Returns an input stream that reads from this open connection.
		''' 
		''' A SocketTimeoutException can be thrown when reading from the
		''' returned input stream if the read timeout expires before data
		''' is available for read.
		''' </summary>
		''' <returns>     an input stream that reads from this open connection. </returns>
		''' <exception cref="IOException">              if an I/O error occurs while
		'''               creating the input stream. </exception>
		''' <exception cref="UnknownServiceException">  if the protocol does not support
		'''               input. </exception>
		''' <seealso cref= #setReadTimeout(int) </seealso>
		''' <seealso cref= #getReadTimeout() </seealso>
		Public Overridable Property inputStream As java.io.InputStream
			Get
				Throw New UnknownServiceException("protocol doesn't support input")
			End Get
		End Property

		''' <summary>
		''' Returns an output stream that writes to this connection.
		''' </summary>
		''' <returns>     an output stream that writes to this connection. </returns>
		''' <exception cref="IOException">              if an I/O error occurs while
		'''               creating the output stream. </exception>
		''' <exception cref="UnknownServiceException">  if the protocol does not support
		'''               output. </exception>
		Public Overridable Property outputStream As java.io.OutputStream
			Get
				Throw New UnknownServiceException("protocol doesn't support output")
			End Get
		End Property

		''' <summary>
		''' Returns a {@code String} representation of this URL connection.
		''' </summary>
		''' <returns>  a string representation of this {@code URLConnection}. </returns>
		Public Overrides Function ToString() As String
			Return Me.GetType().name & ":" & url
		End Function

		''' <summary>
		''' Sets the value of the {@code doInput} field for this
		''' {@code URLConnection} to the specified value.
		''' <p>
		''' A URL connection can be used for input and/or output.  Set the DoInput
		''' flag to true if you intend to use the URL connection for input,
		''' false if not.  The default is true.
		''' </summary>
		''' <param name="doinput">   the new value. </param>
		''' <exception cref="IllegalStateException"> if already connected </exception>
		''' <seealso cref=     java.net.URLConnection#doInput </seealso>
		''' <seealso cref= #getDoInput() </seealso>
		Public Overridable Property doInput As Boolean
			Set(ByVal doinput As Boolean)
				If connected Then Throw New IllegalStateException("Already connected")
				Me.doInput = doinput
			End Set
			Get
				Return doInput
			End Get
		End Property


		''' <summary>
		''' Sets the value of the {@code doOutput} field for this
		''' {@code URLConnection} to the specified value.
		''' <p>
		''' A URL connection can be used for input and/or output.  Set the DoOutput
		''' flag to true if you intend to use the URL connection for output,
		''' false if not.  The default is false.
		''' </summary>
		''' <param name="dooutput">   the new value. </param>
		''' <exception cref="IllegalStateException"> if already connected </exception>
		''' <seealso cref= #getDoOutput() </seealso>
		Public Overridable Property doOutput As Boolean
			Set(ByVal dooutput As Boolean)
				If connected Then Throw New IllegalStateException("Already connected")
				Me.doOutput = dooutput
			End Set
			Get
				Return doOutput
			End Get
		End Property


		''' <summary>
		''' Set the value of the {@code allowUserInteraction} field of
		''' this {@code URLConnection}.
		''' </summary>
		''' <param name="allowuserinteraction">   the new value. </param>
		''' <exception cref="IllegalStateException"> if already connected </exception>
		''' <seealso cref=     #getAllowUserInteraction() </seealso>
		Public Overridable Property allowUserInteraction As Boolean
			Set(ByVal allowuserinteraction As Boolean)
				If connected Then Throw New IllegalStateException("Already connected")
				Me.allowUserInteraction = allowuserinteraction
			End Set
			Get
				Return allowUserInteraction
			End Get
		End Property


		''' <summary>
		''' Sets the default value of the
		''' {@code allowUserInteraction} field for all future
		''' {@code URLConnection} objects to the specified value.
		''' </summary>
		''' <param name="defaultallowuserinteraction">   the new value. </param>
		''' <seealso cref=     #getDefaultAllowUserInteraction() </seealso>
		Public Shared Property defaultAllowUserInteraction As Boolean
			Set(ByVal defaultallowuserinteraction As Boolean)
				URLConnection.defaultAllowUserInteraction = defaultallowuserinteraction
			End Set
			Get
				Return defaultAllowUserInteraction
			End Get
		End Property


		''' <summary>
		''' Sets the value of the {@code useCaches} field of this
		''' {@code URLConnection} to the specified value.
		''' <p>
		''' Some protocols do caching of documents.  Occasionally, it is important
		''' to be able to "tunnel through" and ignore the caches (e.g., the
		''' "reload" button in a browser).  If the UseCaches flag on a connection
		''' is true, the connection is allowed to use whatever caches it can.
		'''  If false, caches are to be ignored.
		'''  The default value comes from DefaultUseCaches, which defaults to
		''' true.
		''' </summary>
		''' <param name="usecaches"> a {@code boolean} indicating whether
		''' or not to allow caching </param>
		''' <exception cref="IllegalStateException"> if already connected </exception>
		''' <seealso cref= #getUseCaches() </seealso>
		Public Overridable Property useCaches As Boolean
			Set(ByVal usecaches As Boolean)
				If connected Then Throw New IllegalStateException("Already connected")
				Me.useCaches = usecaches
			End Set
			Get
				Return useCaches
			End Get
		End Property


		''' <summary>
		''' Sets the value of the {@code ifModifiedSince} field of
		''' this {@code URLConnection} to the specified value.
		''' </summary>
		''' <param name="ifmodifiedsince">   the new value. </param>
		''' <exception cref="IllegalStateException"> if already connected </exception>
		''' <seealso cref=     #getIfModifiedSince() </seealso>
		Public Overridable Property ifModifiedSince As Long
			Set(ByVal ifmodifiedsince As Long)
				If connected Then Throw New IllegalStateException("Already connected")
				Me.ifModifiedSince = ifmodifiedsince
			End Set
			Get
				Return ifModifiedSince
			End Get
		End Property


	   ''' <summary>
	   ''' Returns the default value of a {@code URLConnection}'s
	   ''' {@code useCaches} flag.
	   ''' <p>
	   ''' Ths default is "sticky", being a part of the static state of all
	   ''' URLConnections.  This flag applies to the next, and all following
	   ''' URLConnections that are created.
	   ''' </summary>
	   ''' <returns>  the default value of a {@code URLConnection}'s
	   '''          {@code useCaches} flag. </returns>
	   ''' <seealso cref=     #setDefaultUseCaches(boolean) </seealso>
		Public Overridable Property defaultUseCaches As Boolean
			Get
				Return defaultUseCaches
			End Get
			Set(ByVal defaultusecaches As Boolean)
				URLConnection.defaultUseCaches = defaultusecaches
			End Set
		End Property


		''' <summary>
		''' Sets the general request property. If a property with the key already
		''' exists, overwrite its value with the new value.
		''' 
		''' <p> NOTE: HTTP requires all request properties which can
		''' legally have multiple instances with the same key
		''' to use a comma-separated list syntax which enables multiple
		''' properties to be appended into a single property.
		''' </summary>
		''' <param name="key">     the keyword by which the request is known
		'''                  (e.g., "{@code Accept}"). </param>
		''' <param name="value">   the value associated with it. </param>
		''' <exception cref="IllegalStateException"> if already connected </exception>
		''' <exception cref="NullPointerException"> if key is <CODE>null</CODE> </exception>
		''' <seealso cref= #getRequestProperty(java.lang.String) </seealso>
		Public Overridable Sub setRequestProperty(ByVal key As String, ByVal value As String)
			If connected Then Throw New IllegalStateException("Already connected")
			If key Is Nothing Then Throw New NullPointerException("key is null")

			If requests Is Nothing Then requests = New sun.net.www.MessageHeader

			requests.set(key, value)
		End Sub

		''' <summary>
		''' Adds a general request property specified by a
		''' key-value pair.  This method will not overwrite
		''' existing values associated with the same key.
		''' </summary>
		''' <param name="key">     the keyword by which the request is known
		'''                  (e.g., "{@code Accept}"). </param>
		''' <param name="value">  the value associated with it. </param>
		''' <exception cref="IllegalStateException"> if already connected </exception>
		''' <exception cref="NullPointerException"> if key is null </exception>
		''' <seealso cref= #getRequestProperties()
		''' @since 1.4 </seealso>
		Public Overridable Sub addRequestProperty(ByVal key As String, ByVal value As String)
			If connected Then Throw New IllegalStateException("Already connected")
			If key Is Nothing Then Throw New NullPointerException("key is null")

			If requests Is Nothing Then requests = New sun.net.www.MessageHeader

			requests.add(key, value)
		End Sub


		''' <summary>
		''' Returns the value of the named general request property for this
		''' connection.
		''' </summary>
		''' <param name="key"> the keyword by which the request is known (e.g., "Accept"). </param>
		''' <returns>  the value of the named general request property for this
		'''           connection. If key is null, then null is returned. </returns>
		''' <exception cref="IllegalStateException"> if already connected </exception>
		''' <seealso cref= #setRequestProperty(java.lang.String, java.lang.String) </seealso>
		Public Overridable Function getRequestProperty(ByVal key As String) As String
			If connected Then Throw New IllegalStateException("Already connected")

			If requests Is Nothing Then Return Nothing

			Return requests.findValue(key)
		End Function

		''' <summary>
		''' Returns an unmodifiable Map of general request
		''' properties for this connection. The Map keys
		''' are Strings that represent the request-header
		''' field names. Each Map value is a unmodifiable List
		''' of Strings that represents the corresponding
		''' field values.
		''' </summary>
		''' <returns>  a Map of the general request properties for this connection. </returns>
		''' <exception cref="IllegalStateException"> if already connected
		''' @since 1.4 </exception>
		Public Overridable Property requestProperties As IDictionary(Of String, IList(Of String))
			Get
				If connected Then Throw New IllegalStateException("Already connected")
    
				If requests Is Nothing Then Return java.util.Collections.emptyMap()
    
				Return requests.getHeaders(Nothing)
			End Get
		End Property

		''' <summary>
		''' Sets the default value of a general request property. When a
		''' {@code URLConnection} is created, it is initialized with
		''' these properties.
		''' </summary>
		''' <param name="key">     the keyword by which the request is known
		'''                  (e.g., "{@code Accept}"). </param>
		''' <param name="value">   the value associated with the key.
		''' </param>
		''' <seealso cref= java.net.URLConnection#setRequestProperty(java.lang.String,java.lang.String)
		''' </seealso>
		''' @deprecated The instance specific setRequestProperty method
		''' should be used after an appropriate instance of URLConnection
		''' is obtained. Invoking this method will have no effect.
		''' 
		''' <seealso cref= #getDefaultRequestProperty(java.lang.String) </seealso>
		<Obsolete("The instance specific setRequestProperty method")> _
		Public Shared Sub setDefaultRequestProperty(ByVal key As String, ByVal value As String)
		End Sub

		''' <summary>
		''' Returns the value of the default request property. Default request
		''' properties are set for every connection.
		''' </summary>
		''' <param name="key"> the keyword by which the request is known (e.g., "Accept"). </param>
		''' <returns>  the value of the default request property
		''' for the specified key.
		''' </returns>
		''' <seealso cref= java.net.URLConnection#getRequestProperty(java.lang.String)
		''' </seealso>
		''' @deprecated The instance specific getRequestProperty method
		''' should be used after an appropriate instance of URLConnection
		''' is obtained.
		''' 
		''' <seealso cref= #setDefaultRequestProperty(java.lang.String, java.lang.String) </seealso>
		<Obsolete("The instance specific getRequestProperty method")> _
		Public Shared Function getDefaultRequestProperty(ByVal key As String) As String
			Return Nothing
		End Function

		''' <summary>
		''' The ContentHandler factory.
		''' </summary>
		Friend Shared factory As ContentHandlerFactory

		''' <summary>
		''' Sets the {@code ContentHandlerFactory} of an
		''' application. It can be called at most once by an application.
		''' <p>
		''' The {@code ContentHandlerFactory} instance is used to
		''' construct a content handler from a content type
		''' <p>
		''' If there is a security manager, this method first calls
		''' the security manager's {@code checkSetFactory} method
		''' to ensure the operation is allowed.
		''' This could result in a SecurityException.
		''' </summary>
		''' <param name="fac">   the desired factory. </param>
		''' <exception cref="Error">  if the factory has already been defined. </exception>
		''' <exception cref="SecurityException">  if a security manager exists and its
		'''             {@code checkSetFactory} method doesn't allow the operation. </exception>
		''' <seealso cref=        java.net.ContentHandlerFactory </seealso>
		''' <seealso cref=        java.net.URLConnection#getContent() </seealso>
		''' <seealso cref=        SecurityManager#checkSetFactory </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Shared Property contentHandlerFactory As ContentHandlerFactory
			Set(ByVal fac As ContentHandlerFactory)
				If factory IsNot Nothing Then Throw New [Error]("factory already defined")
				Dim security As SecurityManager = System.securityManager
				If security IsNot Nothing Then security.checkSetFactory()
				factory = fac
			End Set
		End Property

		Private Shared handlers As New Dictionary(Of String, ContentHandler)

		''' <summary>
		''' Gets the Content Handler appropriate for this connection.
		''' </summary>
		SyncLock ContentHandler getContentHandler
			Dim UnknownServiceException As throws
			Dim contentType_Renamed As String = stripOffParameters(contentType)
			Dim handler As ContentHandler = Nothing
			If contentType_Renamed Is Nothing Then Throw New UnknownServiceException("no content-type")
			Try
				handler = handlers(contentType_Renamed)
				If handler IsNot Nothing Then Return handler
			Catch e As Exception
			End Try

			If factory IsNot Nothing Then handler = factory.createContentHandler(contentType_Renamed)
			If handler Is Nothing Then
				Try
					handler = lookupContentHandlerClassFor(contentType_Renamed)
				Catch e As Exception
					e.printStackTrace()
					handler = UnknownContentHandler.INSTANCE
				End Try
				handlers(contentType_Renamed) = handler
			End If
			Return handler
		End SyncLock

	'    
	'     * Media types are in the format: type/subtype*(; parameter).
	'     * For looking up the content handler, we should ignore those
	'     * parameters.
	'     
		private String stripOffParameters(String contentType)
			If contentType Is Nothing Then Return Nothing
			Dim index As Integer = contentType.IndexOf(";"c)

			If index > 0 Then
				Return contentType.Substring(0, index)
			Else
				Return contentType
			End If

		private static final String contentClassPrefix = "sun.net.www.content"
		private static final String contentPathProp = "java.content.handler.pkgs"

		''' <summary>
		''' Looks for a content handler in a user-defineable set of places.
		''' By default it looks in sun.net.www.content, but users can define a
		''' vertical-bar delimited set of class prefixes to search through in
		''' addition by defining the java.content.handler.pkgs property.
		''' The class name must be of the form:
		''' <pre>
		'''     {package-prefix}.{major}.{minor}
		''' e.g.
		'''     YoyoDyne.experimental.text.plain
		''' </pre>
		''' </summary>
		private ContentHandler lookupContentHandlerClassFor(String contentType) throws InstantiationException, IllegalAccessException, ClassNotFoundException
			Dim contentHandlerClassName As String = typeToPackageName(contentType)

			Dim contentHandlerPkgPrefixes_Renamed As String =contentHandlerPkgPrefixes

			Dim packagePrefixIter As New java.util.StringTokenizer(contentHandlerPkgPrefixes_Renamed, "|")

			Do While packagePrefixIter.hasMoreTokens()
				Dim packagePrefix As String = packagePrefixIter.nextToken().Trim()

				Try
					Dim clsName As String = packagePrefix & "." & contentHandlerClassName
					Dim cls As  [Class] = Nothing
					Try
						cls = Type.GetType(clsName)
					Catch e As  [Class]NotFoundException
						Dim cl As  [Class]Loader = ClassLoader.systemClassLoader
						If cl IsNot Nothing Then cls = cl.loadClass(clsName)
					End Try
					If cls IsNot Nothing Then
						Dim handler As ContentHandler = CType(cls.newInstance(), ContentHandler)
						Return handler
					End If
				Catch e As Exception
				End Try
			Loop

			Return UnknownContentHandler.INSTANCE

		''' <summary>
		''' Utility function to map a MIME content type into an equivalent
		''' pair of class name components.  For example: "text/html" would
		''' be returned as "text.html"
		''' </summary>
		private String typeToPackageName(String contentType)
			' make sure we canonicalize the class name: all lower case
			contentType = contentType.ToLower()
			Dim len As Integer = contentType.length()
			Dim nm As Char() = New Char(len - 1){}
			contentType.getChars(0, len, nm, 0)
			For i As Integer = 0 To len - 1
				Dim c As Char = nm(i)
				If c = "/"c Then
					nm(i) = "."c
				ElseIf Not("A"c <= c AndAlso c <= "Z"c OrElse "a"c <= c AndAlso c <= "z"c OrElse "0"c <= c AndAlso c <= "9"c) Then
					nm(i) = "_"c
				End If
			Next i
			Return New String(nm)


		''' <summary>
		''' Returns a vertical bar separated list of package prefixes for potential
		''' content handlers.  Tries to get the java.content.handler.pkgs property
		''' to use as a set of package prefixes to search.  Whether or not
		''' that property has been defined, the sun.net.www.content is always
		''' the last one on the returned package list.
		''' </summary>
		private String contentHandlerPkgPrefixes
			Dim packagePrefixList As String = java.security.AccessController.doPrivileged(New sun.security.action.GetPropertyAction(contentPathProp, ""))

			If packagePrefixList <> "" Then packagePrefixList &= "|"

			Return packagePrefixList + contentClassPrefix

		''' <summary>
		''' Tries to determine the content type of an object, based
		''' on the specified "file" component of a URL.
		''' This is a convenience method that can be used by
		''' subclasses that override the {@code getContentType} method.
		''' </summary>
		''' <param name="fname">   a filename. </param>
		''' <returns>  a guess as to what the content type of the object is,
		'''          based upon its file name. </returns>
		''' <seealso cref=     java.net.URLConnection#getContentType() </seealso>
		public static String guessContentTypeFromName(String fname)
			Return fileNameMap.getContentTypeFor(fname)

		''' <summary>
		''' Tries to determine the type of an input stream based on the
		''' characters at the beginning of the input stream. This method can
		''' be used by subclasses that override the
		''' {@code getContentType} method.
		''' <p>
		''' Ideally, this routine would not be needed. But many
		''' {@code http} servers return the incorrect content type; in
		''' addition, there are many nonstandard extensions. Direct inspection
		''' of the bytes to determine the content type is often more accurate
		''' than believing the content type claimed by the {@code http} server.
		''' </summary>
		''' <param name="is">   an input stream that supports marks. </param>
		''' <returns>     a guess at the content type, or {@code null} if none
		'''             can be determined. </returns>
		''' <exception cref="IOException">  if an I/O error occurs while reading the
		'''               input stream. </exception>
		''' <seealso cref=        java.io.InputStream#mark(int) </seealso>
		''' <seealso cref=        java.io.InputStream#markSupported() </seealso>
		''' <seealso cref=        java.net.URLConnection#getContentType() </seealso>
		public static String guessContentTypeFromStream(java.io.InputStream is) throws java.io.IOException
			' If we can't read ahead safely, just give up on guessing
			If Not is.markSupported() Then Return Nothing

			is.mark(16)
			Dim c1 As Integer = is.read()
			Dim c2 As Integer = is.read()
			Dim c3 As Integer = is.read()
			Dim c4 As Integer = is.read()
			Dim c5 As Integer = is.read()
			Dim c6 As Integer = is.read()
			Dim c7 As Integer = is.read()
			Dim c8 As Integer = is.read()
			Dim c9 As Integer = is.read()
			Dim c10 As Integer = is.read()
			Dim c11 As Integer = is.read()
			Dim c12 As Integer = is.read()
			Dim c13 As Integer = is.read()
			Dim c14 As Integer = is.read()
			Dim c15 As Integer = is.read()
			Dim c16 As Integer = is.read()
			is.reset()

			If c1 = &HCA AndAlso c2 = &HFE AndAlso c3 = &HBA AndAlso c4 = &HBE Then Return "application/java-vm"

			If c1 = &HAC AndAlso c2 = &HED Then Return "application/x-java-serialized-object"

			If c1 = AscW("<"c) Then
				If c2 = AscW("!"c) OrElse ((c2 = AscW("h"c) AndAlso (c3 = AscW("t"c) AndAlso c4 = AscW("m"c) AndAlso c5 = AscW("l"c) OrElse c3 = AscW("e"c) AndAlso c4 = AscW("a"c) AndAlso c5 = AscW("d"c)) OrElse (c2 = AscW("b"c) AndAlso c3 = AscW("o"c) AndAlso c4 = AscW("d"c) AndAlso c5 = AscW("y"c)))) OrElse ((c2 = AscW("H"c) AndAlso (c3 = AscW("T"c) AndAlso c4 = AscW("M"c) AndAlso c5 = AscW("L"c) OrElse c3 = AscW("E"c) AndAlso c4 = AscW("A"c) AndAlso c5 = AscW("D"c)) OrElse (c2 = AscW("B"c) AndAlso c3 = AscW("O"c) AndAlso c4 = AscW("D"c) AndAlso c5 = AscW("Y"c)))) Then Return "text/html"

				If c2 = AscW("?"c) AndAlso c3 = AscW("x"c) AndAlso c4 = AscW("m"c) AndAlso c5 = AscW("l"c) AndAlso c6 = AscW(" "c) Then Return "application/xml"
			End If

			' big and little (identical) endian UTF-8 encodings, with BOM
			If c1 = &Hef AndAlso c2 = &Hbb AndAlso c3 = &Hbf Then
				If c4 = AscW("<"c) AndAlso c5 = AscW("?"c) AndAlso c6 = AscW("x"c) Then Return "application/xml"
			End If

			' big and little endian UTF-16 encodings, with byte order mark
			If c1 = &Hfe AndAlso c2 = &Hff Then
				If c3 = 0 AndAlso c4 = AscW("<"c) AndAlso c5 = 0 AndAlso c6 = AscW("?"c) AndAlso c7 = 0 AndAlso c8 = AscW("x"c) Then Return "application/xml"
			End If

			If c1 = &Hff AndAlso c2 = &Hfe Then
				If c3 = AscW("<"c) AndAlso c4 = 0 AndAlso c5 = AscW("?"c) AndAlso c6 = 0 AndAlso c7 = AscW("x"c) AndAlso c8 = 0 Then Return "application/xml"
			End If

			' big and little endian UTF-32 encodings, with BOM
			If c1 = &H0 AndAlso c2 = &H0 AndAlso c3 = &Hfe AndAlso c4 = &Hff Then
				If c5 = 0 AndAlso c6 = 0 AndAlso c7 = 0 AndAlso c8 = AscW("<"c) AndAlso c9 = 0 AndAlso c10 = 0 AndAlso c11 = 0 AndAlso c12 = AscW("?"c) AndAlso c13 = 0 AndAlso c14 = 0 AndAlso c15 = 0 AndAlso c16 = AscW("x"c) Then Return "application/xml"
			End If

			If c1 = &Hff AndAlso c2 = &Hfe AndAlso c3 = &H0 AndAlso c4 = &H0 Then
				If c5 = AscW("<"c) AndAlso c6 = 0 AndAlso c7 = 0 AndAlso c8 = 0 AndAlso c9 = AscW("?"c) AndAlso c10 = 0 AndAlso c11 = 0 AndAlso c12 = 0 AndAlso c13 = AscW("x"c) AndAlso c14 = 0 AndAlso c15 = 0 AndAlso c16 = 0 Then Return "application/xml"
			End If

			If c1 = AscW("G"c) AndAlso c2 = AscW("I"c) AndAlso c3 = AscW("F"c) AndAlso c4 = AscW("8"c) Then Return "image/gif"

			If c1 = AscW("#"c) AndAlso c2 = AscW("d"c) AndAlso c3 = AscW("e"c) AndAlso c4 = AscW("f"c) Then Return "image/x-bitmap"

			If c1 = AscW("!"c) AndAlso c2 = AscW(" "c) AndAlso c3 = AscW("X"c) AndAlso c4 = AscW("P"c) AndAlso c5 = AscW("M"c) AndAlso c6 = AscW("2"c) Then Return "image/x-pixmap"

			If c1 = 137 AndAlso c2 = 80 AndAlso c3 = 78 AndAlso c4 = 71 AndAlso c5 = 13 AndAlso c6 = 10 AndAlso c7 = 26 AndAlso c8 = 10 Then Return "image/png"

			If c1 = &HFF AndAlso c2 = &HD8 AndAlso c3 = &HFF Then
				If c4 = &HE0 Then Return "image/jpeg"

				''' <summary>
				''' File format used by digital cameras to store images.
				''' Exif Format can be read by any application supporting
				''' JPEG. Exif Spec can be found at:
				''' http://www.pima.net/standards/it10/PIMA15740/Exif_2-1.PDF
				''' </summary>
				If (c4 = &HE1) AndAlso (c7 = AscW("E"c) AndAlso c8 = AscW("x"c) AndAlso c9 = AscW("i"c) AndAlso c10 =AscW("f"c) AndAlso c11 = 0) Then Return "image/jpeg"

				If c4 = &HEE Then Return "image/jpg"
			End If

			If c1 = &HD0 AndAlso c2 = &HCF AndAlso c3 = &H11 AndAlso c4 = &HE0 AndAlso c5 = &HA1 AndAlso c6 = &HB1 AndAlso c7 = &H1A AndAlso c8 = &HE1 Then

	'             Above is signature of Microsoft Structured Storage.
	'             * Below this, could have tests for various SS entities.
	'             * For now, just test for FlashPix.
	'             
				If checkfpx(is) Then Return "image/vnd.fpx"
			End If

			If c1 = &H2E AndAlso c2 = &H73 AndAlso c3 = &H6E AndAlso c4 = &H64 Then Return "audio/basic" ' .au format, big endian

			If c1 = &H64 AndAlso c2 = &H6E AndAlso c3 = &H73 AndAlso c4 = &H2E Then Return "audio/basic" ' .au format, little endian

			If c1 = AscW("R"c) AndAlso c2 = AscW("I"c) AndAlso c3 = AscW("F"c) AndAlso c4 = AscW("F"c) Then Return "audio/x-wav"
			Return Nothing

		''' <summary>
		''' Check for FlashPix image data in InputStream is.  Return true if
		''' the stream has FlashPix data, false otherwise.  Before calling this
		''' method, the stream should have already been checked to be sure it
		''' contains Microsoft Structured Storage data.
		''' </summary>
		private static Boolean checkfpx(java.io.InputStream is) throws java.io.IOException

	'         Test for FlashPix image data in Microsoft Structured Storage format.
	'         * In general, should do this with calls to an SS implementation.
	'         * Lacking that, need to dig via offsets to get to the FlashPix
	'         * ClassID.  Details:
	'         *
	'         * Offset to Fpx ClsID from beginning of stream should be:
	'         *
	'         * FpxClsidOffset = rootEntryOffset + clsidOffset
	'         *
	'         * where: clsidOffset = 0x50.
	'         *        rootEntryOffset = headerSize + sectorSize*sectDirStart
	'         *                          + 128*rootEntryDirectory
	'         *
	'         *        where:  headerSize = 0x200 (always)
	'         *                sectorSize = 2 raised to power of uSectorShift,
	'         *                             which is found in the header at
	'         *                             offset 0x1E.
	'         *                sectDirStart = found in the header at offset 0x30.
	'         *                rootEntryDirectory = in general, should search for
	'         *                                     directory labelled as root.
	'         *                                     We will assume value of 0 (i.e.,
	'         *                                     rootEntry is in first directory)
	'         

			' Mark the stream so we can reset it. 0x100 is enough for the first
			' few reads, but the mark will have to be reset and set again once
			' the offset to the root directory entry is computed. That offset
			' can be very large and isn't know until the stream has been read from
			is.mark(&H100)

			' Get the byte ordering located at 0x1E. 0xFE is Intel,
			' 0xFF is other
			Dim toSkip As Long = CLng(&H1C)
			Dim posn As Long

			posn = skipForward(is, toSkip)
			If posn < toSkip Then
			  is.reset()
			  Return False
			End If

			Dim c As Integer() = New Integer(15){}
			If readBytes(c, 2, is) < 0 Then
				is.reset()
				Return False
			End If

			Dim byteOrder As Integer = c(0)

			posn+=2
			Dim uSectorShift As Integer
			If readBytes(c, 2, is) < 0 Then
				is.reset()
				Return False
			End If

			If byteOrder = &HFE Then
				uSectorShift = c(0)
				uSectorShift += c(1) << 8
			Else
				uSectorShift = c(0) << 8
				uSectorShift += c(1)
			End If

			posn += 2
			toSkip = CLng(&H30) - posn
			Dim skipped As Long = 0
			skipped = skipForward(is, toSkip)
			If skipped < toSkip Then
			  is.reset()
			  Return False
			End If
			posn += skipped

			If readBytes(c, 4, is) < 0 Then
				is.reset()
				Return False
			End If

			Dim sectDirStart As Integer
			If byteOrder = &HFE Then
				sectDirStart = c(0)
				sectDirStart += c(1) << 8
				sectDirStart += c(2) << 16
				sectDirStart += c(3) << 24
			Else
				sectDirStart = c(0) << 24
				sectDirStart += c(1) << 16
				sectDirStart += c(2) << 8
				sectDirStart += c(3)
			End If
			posn += 4
			is.reset() ' Reset back to the beginning

			toSkip = &H200L + CLng(Fix(1<<uSectorShift))*sectDirStart + &H50L

			' Sanity check!
			If toSkip < 0 Then Return False

	'        
	'         * How far can we skip? Is there any performance problem here?
	'         * This skip can be fairly long, at least 0x4c650 in at least
	'         * one case. Have to assume that the skip will fit in an int.
	'         * Leave room to read whole root dir
	'         
			is.mark(CInt(toSkip)+&H30)

			If (skipForward(is, toSkip)) < toSkip Then
				is.reset()
				Return False
			End If

	'         should be at beginning of ClassID, which is as follows
	'         * (in Intel byte order):
	'         *    00 67 61 56 54 C1 CE 11 85 53 00 AA 00 A1 F9 5B
	'         *
	'         * This is stored from Windows as long,short,short,char[8]
	'         * so for byte order changes, the order only changes for
	'         * the first 8 bytes in the ClassID.
	'         *
	'         * Test against this, ignoring second byte (Intel) since
	'         * this could change depending on part of Fpx file we have.
	'         

			If readBytes(c, 16, is) < 0 Then
				is.reset()
				Return False
			End If

			' intel byte order
			If byteOrder = &HFE AndAlso c(0) = &H0 AndAlso c(2) = &H61 AndAlso c(3) = &H56 AndAlso c(4) = &H54 AndAlso c(5) = &HC1 AndAlso c(6) = &HCE AndAlso c(7) = &H11 AndAlso c(8) = &H85 AndAlso c(9) = &H53 AndAlso c(10)= &H0 AndAlso c(11)= &HAA AndAlso c(12)= &H0 AndAlso c(13)= &HA1 AndAlso c(14)= &HF9 AndAlso c(15)= &H5B Then
				is.reset()
				Return True

			' non-intel byte order
			ElseIf c(3) = &H0 AndAlso c(1) = &H61 AndAlso c(0) = &H56 AndAlso c(5) = &H54 AndAlso c(4) = &HC1 AndAlso c(7) = &HCE AndAlso c(6) = &H11 AndAlso c(8) = &H85 AndAlso c(9) = &H53 AndAlso c(10)= &H0 AndAlso c(11)= &HAA AndAlso c(12)= &H0 AndAlso c(13)= &HA1 AndAlso c(14)= &HF9 AndAlso c(15)= &H5B Then
				is.reset()
				Return True
			End If
			is.reset()
			Return False

		''' <summary>
		''' Tries to read the specified number of bytes from the stream
		''' Returns -1, If EOF is reached before len bytes are read, returns 0
		''' otherwise
		''' </summary>
		private static Integer readBytes(Integer c() , Integer len, java.io.InputStream is) throws java.io.IOException

			Dim buf As SByte() = New SByte(len - 1){}
			If is.read(buf, 0, len) < len Then Return -1

			' fill the passed in int array
			For i As Integer = 0 To len - 1
				 c(i) = buf(i) And &Hff
			Next i
			Return 0


		''' <summary>
		''' Skips through the specified number of bytes from the stream
		''' until either EOF is reached, or the specified
		''' number of bytes have been skipped
		''' </summary>
		private static Long skipForward(java.io.InputStream is, Long toSkip) throws java.io.IOException

			Dim eachSkip As Long = 0
			Dim skipped As Long = 0

			Do While skipped <> toSkip
				eachSkip = is.skip(toSkip - skipped)

				' check if EOF is reached
				If eachSkip <= 0 Then
					If is.read() = -1 Then
						Return skipped
					Else
						skipped += 1
					End If
				End If
				skipped += eachSkip
			Loop
			Return skipped

	End Class


'JAVA TO VB CONVERTER TODO TASK: Local classes are not converted by Java to VB Converter:
'	class UnknownContentHandler extends ContentHandler
	'{
	'	static final ContentHandler INSTANCE = New UnknownContentHandler();
	'
	'	public Object getContent(URLConnection uc) throws IOException
	'	{
	'		Return uc.getInputStream();
	'	}
	'}

End Namespace