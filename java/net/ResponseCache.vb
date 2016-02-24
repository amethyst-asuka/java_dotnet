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
	''' Represents implementations of URLConnection caches. An instance of
	''' such a class can be registered with the system by doing
	''' ResponseCache.setDefault(ResponseCache), and the system will call
	''' this object in order to:
	''' 
	'''    <ul><li>store resource data which has been retrieved from an
	'''            external source into the cache</li>
	'''         <li>try to fetch a requested resource that may have been
	'''            stored in the cache</li>
	'''    </ul>
	''' 
	''' The ResponseCache implementation decides which resources
	''' should be cached, and for how long they should be cached. If a
	''' request resource cannot be retrieved from the cache, then the
	''' protocol handlers will fetch the resource from its original
	''' location.
	''' 
	''' The settings for URLConnection#useCaches controls whether the
	''' protocol is allowed to use a cached response.
	''' 
	''' For more information on HTTP caching, see <a
	''' href="http://www.ietf.org/rfc/rfc2616.txt"><i>RFC&nbsp;2616: Hypertext
	''' Transfer Protocol -- HTTP/1.1</i></a>
	''' 
	''' @author Yingxian Wang
	''' @since 1.5
	''' </summary>
	Public MustInherit Class ResponseCache

		''' <summary>
		''' The system wide cache that provides access to a url
		''' caching mechanism.
		''' </summary>
		''' <seealso cref= #setDefault(ResponseCache) </seealso>
		''' <seealso cref= #getDefault() </seealso>
		Private Shared theResponseCache As ResponseCache

		''' <summary>
		''' Gets the system-wide response cache.
		''' </summary>
		''' <exception cref="SecurityException">
		'''          If a security manager has been installed and it denies
		''' <seealso cref="NetPermission"/>{@code ("getResponseCache")}
		''' </exception>
		''' <seealso cref= #setDefault(ResponseCache) </seealso>
		''' <returns> the system-wide {@code ResponseCache}
		''' @since 1.5 </returns>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Property Shared [default] As ResponseCache
			Get
				Dim sm As SecurityManager = System.securityManager
				If sm IsNot Nothing Then sm.checkPermission(sun.security.util.SecurityConstants.GET_RESPONSECACHE_PERMISSION)
				Return theResponseCache
			End Get
			Set(ByVal responseCache_Renamed As ResponseCache)
				Dim sm As SecurityManager = System.securityManager
				If sm IsNot Nothing Then sm.checkPermission(sun.security.util.SecurityConstants.SET_RESPONSECACHE_PERMISSION)
				theResponseCache = responseCache_Renamed
			End Set
		End Property


		''' <summary>
		''' Retrieve the cached response based on the requesting uri,
		''' request method and request headers. Typically this method is
		''' called by the protocol handler before it sends out the request
		''' to get the network resource. If a cached response is returned,
		''' that resource is used instead.
		''' </summary>
		''' <param name="uri"> a {@code URI} used to reference the requested
		'''            network resource </param>
		''' <param name="rqstMethod"> a {@code String} representing the request
		'''            method </param>
		''' <param name="rqstHeaders"> - a Map from request header
		'''            field names to lists of field values representing
		'''            the current request headers </param>
		''' <returns> a {@code CacheResponse} instance if available
		'''          from cache, or null otherwise </returns>
		''' <exception cref="IOException"> if an I/O error occurs </exception>
		''' <exception cref="IllegalArgumentException"> if any one of the arguments is null
		''' </exception>
		''' <seealso cref=     java.net.URLConnection#setUseCaches(boolean) </seealso>
		''' <seealso cref=     java.net.URLConnection#getUseCaches() </seealso>
		''' <seealso cref=     java.net.URLConnection#setDefaultUseCaches(boolean) </seealso>
		''' <seealso cref=     java.net.URLConnection#getDefaultUseCaches() </seealso>
		Public MustOverride Function [get](ByVal uri As URI, ByVal rqstMethod As String, ByVal rqstHeaders As IDictionary(Of String, IList(Of String))) As CacheResponse

		''' <summary>
		''' The protocol handler calls this method after a resource has
		''' been retrieved, and the ResponseCache must decide whether or
		''' not to store the resource in its cache. If the resource is to
		''' be cached, then put() must return a CacheRequest object which
		''' contains an OutputStream that the protocol handler will
		''' use to write the resource into the cache. If the resource is
		''' not to be cached, then put must return null.
		''' </summary>
		''' <param name="uri"> a {@code URI} used to reference the requested
		'''            network resource </param>
		''' <param name="conn"> - a URLConnection instance that is used to fetch
		'''            the response to be cached </param>
		''' <returns> a {@code CacheRequest} for recording the
		'''            response to be cached. Null return indicates that
		'''            the caller does not intend to cache the response. </returns>
		''' <exception cref="IOException"> if an I/O error occurs </exception>
		''' <exception cref="IllegalArgumentException"> if any one of the arguments is
		'''            null </exception>
		Public MustOverride Function put(ByVal uri As URI, ByVal conn As URLConnection) As CacheRequest
	End Class

End Namespace