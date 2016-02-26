Imports System
Imports System.Runtime.CompilerServices
Imports System.Collections.Generic

'
' * Copyright (c) 2003, 2008, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.rmi.ssl


	''' <summary>
	''' <p>An <code>SslRMIServerSocketFactory</code> instance is used by the RMI
	''' runtime in order to obtain server sockets for RMI calls via SSL.</p>
	''' 
	''' <p>This class implements <code>RMIServerSocketFactory</code> over
	''' the Secure Sockets Layer (SSL) or Transport Layer Security (TLS)
	''' protocols.</p>
	''' 
	''' <p>This class creates SSL sockets using the default
	''' <code>SSLSocketFactory</code> (see {@link
	''' SSLSocketFactory#getDefault}) or the default
	''' <code>SSLServerSocketFactory</code> (see {@link
	''' SSLServerSocketFactory#getDefault}) unless the
	''' constructor taking an <code>SSLContext</code> is
	''' used in which case the SSL sockets are created using
	''' the <code>SSLSocketFactory</code> returned by
	''' <seealso cref="SSLContext#getSocketFactory"/> or the
	''' <code>SSLServerSocketFactory</code> returned by
	''' <seealso cref="SSLContext#getServerSocketFactory"/>.
	''' 
	''' When an <code>SSLContext</code> is not supplied all the instances of this
	''' class share the same keystore, and the same truststore (when client
	''' authentication is required by the server). This behavior can be modified
	''' by supplying an already initialized <code>SSLContext</code> instance.
	''' </summary>
	''' <seealso cref= javax.net.ssl.SSLSocketFactory </seealso>
	''' <seealso cref= javax.net.ssl.SSLServerSocketFactory </seealso>
	''' <seealso cref= javax.rmi.ssl.SslRMIClientSocketFactory
	''' @since 1.5 </seealso>
	Public Class SslRMIServerSocketFactory
		Implements java.rmi.server.RMIServerSocketFactory

		''' <summary>
		''' <p>Creates a new <code>SslRMIServerSocketFactory</code> with
		''' the default SSL socket configuration.</p>
		''' 
		''' <p>SSL connections accepted by server sockets created by this
		''' factory have the default cipher suites and protocol versions
		''' enabled and do not require client authentication.</p>
		''' </summary>
		Public Sub New()
			Me.New(Nothing, Nothing, False)
		End Sub

		''' <summary>
		''' <p>Creates a new <code>SslRMIServerSocketFactory</code> with
		''' the specified SSL socket configuration.</p>
		''' </summary>
		''' <param name="enabledCipherSuites"> names of all the cipher suites to
		''' enable on SSL connections accepted by server sockets created by
		''' this factory, or <code>null</code> to use the cipher suites
		''' that are enabled by default
		''' </param>
		''' <param name="enabledProtocols"> names of all the protocol versions to
		''' enable on SSL connections accepted by server sockets created by
		''' this factory, or <code>null</code> to use the protocol versions
		''' that are enabled by default
		''' </param>
		''' <param name="needClientAuth"> <code>true</code> to require client
		''' authentication on SSL connections accepted by server sockets
		''' created by this factory; <code>false</code> to not require
		''' client authentication
		''' </param>
		''' <exception cref="IllegalArgumentException"> when one or more of the cipher
		''' suites named by the <code>enabledCipherSuites</code> parameter is
		''' not supported, when one or more of the protocols named by the
		''' <code>enabledProtocols</code> parameter is not supported or when
		''' a problem is encountered while trying to check if the supplied
		''' cipher suites and protocols to be enabled are supported.
		''' </exception>
		''' <seealso cref= SSLSocket#setEnabledCipherSuites </seealso>
		''' <seealso cref= SSLSocket#setEnabledProtocols </seealso>
		''' <seealso cref= SSLSocket#setNeedClientAuth </seealso>
		Public Sub New(ByVal enabledCipherSuites As String(), ByVal enabledProtocols As String(), ByVal needClientAuth As Boolean)
			Me.New(Nothing, enabledCipherSuites, enabledProtocols, needClientAuth)
		End Sub

		''' <summary>
		''' <p>Creates a new <code>SslRMIServerSocketFactory</code> with the
		''' specified <code>SSLContext</code> and SSL socket configuration.</p>
		''' </summary>
		''' <param name="context"> the SSL context to be used for creating SSL sockets.
		''' If <code>context</code> is null the default <code>SSLSocketFactory</code>
		''' or the default <code>SSLServerSocketFactory</code> will be used to
		''' create SSL sockets. Otherwise, the socket factory returned by
		''' <code>SSLContext.getSocketFactory()</code> or
		''' <code>SSLContext.getServerSocketFactory()</code> will be used instead.
		''' </param>
		''' <param name="enabledCipherSuites"> names of all the cipher suites to
		''' enable on SSL connections accepted by server sockets created by
		''' this factory, or <code>null</code> to use the cipher suites
		''' that are enabled by default
		''' </param>
		''' <param name="enabledProtocols"> names of all the protocol versions to
		''' enable on SSL connections accepted by server sockets created by
		''' this factory, or <code>null</code> to use the protocol versions
		''' that are enabled by default
		''' </param>
		''' <param name="needClientAuth"> <code>true</code> to require client
		''' authentication on SSL connections accepted by server sockets
		''' created by this factory; <code>false</code> to not require
		''' client authentication
		''' </param>
		''' <exception cref="IllegalArgumentException"> when one or more of the cipher
		''' suites named by the <code>enabledCipherSuites</code> parameter is
		''' not supported, when one or more of the protocols named by the
		''' <code>enabledProtocols</code> parameter is not supported or when
		''' a problem is encountered while trying to check if the supplied
		''' cipher suites and protocols to be enabled are supported.
		''' </exception>
		''' <seealso cref= SSLSocket#setEnabledCipherSuites </seealso>
		''' <seealso cref= SSLSocket#setEnabledProtocols </seealso>
		''' <seealso cref= SSLSocket#setNeedClientAuth
		''' @since 1.7 </seealso>
		Public Sub New(ByVal context As javax.net.ssl.SSLContext, ByVal enabledCipherSuites As String(), ByVal enabledProtocols As String(), ByVal needClientAuth As Boolean)
			' Initialize the configuration parameters.
			'
			Me.enabledCipherSuites = If(enabledCipherSuites Is Nothing, Nothing, enabledCipherSuites.clone())
			Me.enabledProtocols = If(enabledProtocols Is Nothing, Nothing, enabledProtocols.clone())
			Me.needClientAuth = needClientAuth

			' Force the initialization of the default at construction time,
			' rather than delaying it to the first time createServerSocket()
			' is called.
			'
			Me.context = context
			Dim sslSocketFactory As javax.net.ssl.SSLSocketFactory = If(context Is Nothing, defaultSSLSocketFactory, context.socketFactory)
			Dim sslSocket As javax.net.ssl.SSLSocket = Nothing
			If Me.enabledCipherSuites IsNot Nothing OrElse Me.enabledProtocols IsNot Nothing Then
				Try
					sslSocket = CType(sslSocketFactory.createSocket(), javax.net.ssl.SSLSocket)
				Catch e As Exception
					Dim msg As String = "Unable to check if the cipher suites " & "and protocols to enable are supported"
					Throw CType((New System.ArgumentException(msg)).initCause(e), System.ArgumentException)
				End Try
			End If

			' Check if all the cipher suites and protocol versions to enable
			' are supported by the underlying SSL/TLS implementation and if
			' true create lists from arrays.
			'
			If Me.enabledCipherSuites IsNot Nothing Then
				sslSocket.enabledCipherSuites = Me.enabledCipherSuites
				enabledCipherSuitesList = java.util.Arrays.asList(Me.enabledCipherSuites)
			End If
			If Me.enabledProtocols IsNot Nothing Then
				sslSocket.enabledProtocols = Me.enabledProtocols
				enabledProtocolsList = java.util.Arrays.asList(Me.enabledProtocols)
			End If
		End Sub

		''' <summary>
		''' <p>Returns the names of the cipher suites enabled on SSL
		''' connections accepted by server sockets created by this factory,
		''' or <code>null</code> if this factory uses the cipher suites
		''' that are enabled by default.</p>
		''' </summary>
		''' <returns> an array of cipher suites enabled, or <code>null</code>
		''' </returns>
		''' <seealso cref= SSLSocket#setEnabledCipherSuites </seealso>
		Public Property enabledCipherSuites As String()
			Get
				Return If(enabledCipherSuites Is Nothing, Nothing, enabledCipherSuites.clone())
			End Get
		End Property

		''' <summary>
		''' <p>Returns the names of the protocol versions enabled on SSL
		''' connections accepted by server sockets created by this factory,
		''' or <code>null</code> if this factory uses the protocol versions
		''' that are enabled by default.</p>
		''' </summary>
		''' <returns> an array of protocol versions enabled, or
		''' <code>null</code>
		''' </returns>
		''' <seealso cref= SSLSocket#setEnabledProtocols </seealso>
		Public Property enabledProtocols As String()
			Get
				Return If(enabledProtocols Is Nothing, Nothing, enabledProtocols.clone())
			End Get
		End Property

		''' <summary>
		''' <p>Returns <code>true</code> if client authentication is
		''' required on SSL connections accepted by server sockets created
		''' by this factory.</p>
		''' </summary>
		''' <returns> <code>true</code> if client authentication is required
		''' </returns>
		''' <seealso cref= SSLSocket#setNeedClientAuth </seealso>
		Public Property needClientAuth As Boolean
			Get
				Return needClientAuth
			End Get
		End Property

		''' <summary>
		''' <p>Creates a server socket that accepts SSL connections
		''' configured according to this factory's SSL socket configuration
		''' parameters.</p>
		''' </summary>
		Public Overridable Function createServerSocket(ByVal port As Integer) As java.net.ServerSocket
			Dim sslSocketFactory As javax.net.ssl.SSLSocketFactory = If(context Is Nothing, defaultSSLSocketFactory, context.socketFactory)
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'			Return New java.net.ServerSocket(port)
	'		{
	'			public Socket accept() throws IOException
	'			{
	'				Socket socket = MyBase.accept();
	'				SSLSocket sslSocket = (SSLSocket) sslSocketFactory.createSocket(socket, socket.getInetAddress().getHostName(), socket.getPort(), True);
	'				sslSocket.setUseClientMode(False);
	'				if (enabledCipherSuites != Nothing)
	'				{
	'					sslSocket.setEnabledCipherSuites(enabledCipherSuites);
	'				}
	'				if (enabledProtocols != Nothing)
	'				{
	'					sslSocket.setEnabledProtocols(enabledProtocols);
	'				}
	'				sslSocket.setNeedClientAuth(needClientAuth);
	'				Return sslSocket;
	'			}
	'		};
		End Function

		''' <summary>
		''' <p>Indicates whether some other object is "equal to" this one.</p>
		''' 
		''' <p>Two <code>SslRMIServerSocketFactory</code> objects are equal
		''' if they have been constructed with the same SSL context and
		''' SSL socket configuration parameters.</p>
		''' 
		''' <p>A subclass should override this method (as well as
		''' <seealso cref="#hashCode()"/>) if it adds instance state that affects
		''' equality.</p>
		''' </summary>
		Public Overrides Function Equals(ByVal obj As Object) As Boolean
			If obj Is Nothing Then Return False
			If obj Is Me Then Return True
			If Not(TypeOf obj Is SslRMIServerSocketFactory) Then Return False
			Dim that As SslRMIServerSocketFactory = CType(obj, SslRMIServerSocketFactory)
			Return (Me.GetType().Equals(that.GetType()) AndAlso checkParameters(that))
		End Function

		Private Function checkParameters(ByVal that As SslRMIServerSocketFactory) As Boolean
			' SSL context
			'
			If If(context Is Nothing, that.context IsNot Nothing, (Not context.Equals(that.context))) Then Return False

			' needClientAuth flag
			'
			If needClientAuth <> that.needClientAuth Then Return False

			' enabledCipherSuites
			'
			If (enabledCipherSuites Is Nothing AndAlso that.enabledCipherSuites IsNot Nothing) OrElse (enabledCipherSuites IsNot Nothing AndAlso that.enabledCipherSuites Is Nothing) Then Return False
			If enabledCipherSuites IsNot Nothing AndAlso that.enabledCipherSuites IsNot Nothing Then
				Dim thatEnabledCipherSuitesList As IList(Of String) = java.util.Arrays.asList(that.enabledCipherSuites)
				If Not enabledCipherSuitesList.Equals(thatEnabledCipherSuitesList) Then Return False
			End If

			' enabledProtocols
			'
			If (enabledProtocols Is Nothing AndAlso that.enabledProtocols IsNot Nothing) OrElse (enabledProtocols IsNot Nothing AndAlso that.enabledProtocols Is Nothing) Then Return False
			If enabledProtocols IsNot Nothing AndAlso that.enabledProtocols IsNot Nothing Then
				Dim thatEnabledProtocolsList As IList(Of String) = java.util.Arrays.asList(that.enabledProtocols)
				If Not enabledProtocolsList.Equals(thatEnabledProtocolsList) Then Return False
			End If

			Return True
		End Function

		''' <summary>
		''' <p>Returns a hash code value for this
		''' <code>SslRMIServerSocketFactory</code>.</p>
		''' </summary>
		''' <returns> a hash code value for this
		''' <code>SslRMIServerSocketFactory</code>. </returns>
		Public Overrides Function GetHashCode() As Integer
			Return Me.GetType().GetHashCode() + (If(context Is Nothing, 0, context.GetHashCode())) + (If(needClientAuth, Boolean.TRUE.GetHashCode(), Boolean.FALSE.GetHashCode())) + (If(enabledCipherSuites Is Nothing, 0, enabledCipherSuitesList.GetHashCode())) + (If(enabledProtocols Is Nothing, 0, enabledProtocolsList.GetHashCode()))
		End Function

		' We use a static field because:
		'
		'    SSLSocketFactory.getDefault() always returns the same object
		'    (at least on Sun's implementation), and we want to make sure
		'    that the Javadoc & the implementation stay in sync.
		'
		' If someone needs to have different SslRMIServerSocketFactory
		' factories with different underlying SSLSocketFactory objects
		' using different keystores and truststores, he/she can always
		' use the constructor that takes an SSLContext as input.
		'
		Private Shared defaultSSLSocketFactory As javax.net.ssl.SSLSocketFactory = Nothing

		<MethodImpl(MethodImplOptions.Synchronized)> _
		Private Property Shared defaultSSLSocketFactory As javax.net.ssl.SSLSocketFactory
			Get
				If defaultSSLSocketFactory Is Nothing Then defaultSSLSocketFactory = CType(javax.net.ssl.SSLSocketFactory.default, javax.net.ssl.SSLSocketFactory)
				Return defaultSSLSocketFactory
			End Get
		End Property

		Private ReadOnly enabledCipherSuites As String()
		Private ReadOnly enabledProtocols As String()
		Private ReadOnly needClientAuth As Boolean
		Private enabledCipherSuitesList As IList(Of String)
		Private enabledProtocolsList As IList(Of String)
		Private context As javax.net.ssl.SSLContext
	End Class

End Namespace