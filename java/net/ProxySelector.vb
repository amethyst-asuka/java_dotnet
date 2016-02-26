Imports System
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
	''' Selects the proxy server to use, if any, when connecting to the
	''' network resource referenced by a URL. A proxy selector is a
	''' concrete sub-class of this class and is registered by invoking the
	''' <seealso cref="java.net.ProxySelector#setDefault setDefault"/> method. The
	''' currently registered proxy selector can be retrieved by calling
	''' <seealso cref="java.net.ProxySelector#getDefault getDefault"/> method.
	''' 
	''' <p> When a proxy selector is registered, for instance, a subclass
	''' of URLConnection class should call the <seealso cref="#select select"/>
	''' method for each URL request so that the proxy selector can decide
	''' if a direct, or proxied connection should be used. The {@link
	''' #select select} method returns an iterator over a collection with
	''' the preferred connection approach.
	''' 
	''' <p> If a connection cannot be established to a proxy (PROXY or
	''' SOCKS) servers then the caller should call the proxy selector's
	''' <seealso cref="#connectFailed connectFailed"/> method to notify the proxy
	''' selector that the proxy server is unavailable. </p>
	''' 
	''' <P>The default proxy selector does enforce a
	''' <a href="doc-files/net-properties.html#Proxies">set of System Properties</a>
	''' related to proxy settings.</P>
	''' 
	''' @author Yingxian Wang
	''' @author Jean-Christophe Collet
	''' @since 1.5
	''' </summary>
	Public MustInherit Class ProxySelector
		''' <summary>
		''' The system wide proxy selector that selects the proxy server to
		''' use, if any, when connecting to a remote object referenced by
		''' an URL.
		''' </summary>
		''' <seealso cref= #setDefault(ProxySelector) </seealso>
		Private Shared theProxySelector As ProxySelector

		Shared Sub New()
			Try
				Dim c As  [Class] = Type.GetType("sun.net.spi.DefaultProxySelector")
				If c IsNot Nothing AndAlso c.IsSubclassOf(GetType(ProxySelector)) Then theProxySelector = CType(c.newInstance(), ProxySelector)
			Catch e As Exception
				theProxySelector = Nothing
			End Try
		End Sub

		''' <summary>
		''' Gets the system-wide proxy selector.
		''' </summary>
		''' <exception cref="SecurityException">
		'''          If a security manager has been installed and it denies
		''' <seealso cref="NetPermission"/>{@code ("getProxySelector")} </exception>
		''' <seealso cref= #setDefault(ProxySelector) </seealso>
		''' <returns> the system-wide {@code ProxySelector}
		''' @since 1.5 </returns>
		Public Property Shared [default] As ProxySelector
			Get
				Dim sm As SecurityManager = System.securityManager
				If sm IsNot Nothing Then sm.checkPermission(sun.security.util.SecurityConstants.GET_PROXYSELECTOR_PERMISSION)
				Return theProxySelector
			End Get
			Set(ByVal ps As ProxySelector)
				Dim sm As SecurityManager = System.securityManager
				If sm IsNot Nothing Then sm.checkPermission(sun.security.util.SecurityConstants.SET_PROXYSELECTOR_PERMISSION)
				theProxySelector = ps
			End Set
		End Property


		''' <summary>
		''' Selects all the applicable proxies based on the protocol to
		''' access the resource with and a destination address to access
		''' the resource at.
		''' The format of the URI is defined as follow:
		''' <UL>
		''' <LI>http URI for http connections</LI>
		''' <LI>https URI for https connections
		''' <LI>{@code socket://host:port}<br>
		'''     for tcp client sockets connections</LI>
		''' </UL>
		''' </summary>
		''' <param name="uri">
		'''          The URI that a connection is required to
		''' </param>
		''' <returns>  a List of Proxies. Each element in the
		'''          the List is of type
		'''          <seealso cref="java.net.Proxy Proxy"/>;
		'''          when no proxy is available, the list will
		'''          contain one element of type
		'''          <seealso cref="java.net.Proxy Proxy"/>
		'''          that represents a direct connection. </returns>
		''' <exception cref="IllegalArgumentException"> if the argument is null </exception>
		Public MustOverride Function [select](ByVal uri As URI) As IList(Of Proxy)

		''' <summary>
		''' Called to indicate that a connection could not be established
		''' to a proxy/socks server. An implementation of this method can
		''' temporarily remove the proxies or reorder the sequence of
		''' proxies returned by <seealso cref="#select(URI)"/>, using the address
		''' and the IOException caught when trying to connect.
		''' </summary>
		''' <param name="uri">
		'''          The URI that the proxy at sa failed to serve. </param>
		''' <param name="sa">
		'''          The socket address of the proxy/SOCKS server
		''' </param>
		''' <param name="ioe">
		'''          The I/O exception thrown when the connect failed. </param>
		''' <exception cref="IllegalArgumentException"> if either argument is null </exception>
		Public MustOverride Sub connectFailed(ByVal uri As URI, ByVal sa As SocketAddress, ByVal ioe As java.io.IOException)
	End Class

End Namespace