Imports System
Imports System.Runtime.CompilerServices

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
	''' <p>An <code>SslRMIClientSocketFactory</code> instance is used by the RMI
	''' runtime in order to obtain client sockets for RMI calls via SSL.</p>
	''' 
	''' <p>This class implements <code>RMIClientSocketFactory</code> over
	''' the Secure Sockets Layer (SSL) or Transport Layer Security (TLS)
	''' protocols.</p>
	''' 
	''' <p>This class creates SSL sockets using the default
	''' <code>SSLSocketFactory</code> (see {@link
	''' SSLSocketFactory#getDefault}).  All instances of this class are
	''' functionally equivalent.  In particular, they all share the same
	''' truststore, and the same keystore when client authentication is
	''' required by the server.  This behavior can be modified in
	''' subclasses by overriding the <seealso cref="#createSocket(String,int)"/>
	''' method; in that case, <seealso cref="#equals(Object) equals"/> and {@link
	''' #hashCode() hashCode} may also need to be overridden.</p>
	''' 
	''' <p>If the system property
	''' <code>javax.rmi.ssl.client.enabledCipherSuites</code> is specified,
	''' the <seealso cref="#createSocket(String,int)"/> method will call {@link
	''' SSLSocket#setEnabledCipherSuites(String[])} before returning the
	''' socket.  The value of this system property is a string that is a
	''' comma-separated list of SSL/TLS cipher suites to enable.</p>
	''' 
	''' <p>If the system property
	''' <code>javax.rmi.ssl.client.enabledProtocols</code> is specified,
	''' the <seealso cref="#createSocket(String,int)"/> method will call {@link
	''' SSLSocket#setEnabledProtocols(String[])} before returning the
	''' socket.  The value of this system property is a string that is a
	''' comma-separated list of SSL/TLS protocol versions to enable.</p>
	''' </summary>
	''' <seealso cref= javax.net.ssl.SSLSocketFactory </seealso>
	''' <seealso cref= javax.rmi.ssl.SslRMIServerSocketFactory
	''' @since 1.5 </seealso>
	<Serializable> _
	Public Class SslRMIClientSocketFactory
		Implements java.rmi.server.RMIClientSocketFactory

		''' <summary>
		''' <p>Creates a new <code>SslRMIClientSocketFactory</code>.</p>
		''' </summary>
		Public Sub New()
			' We don't force the initialization of the default SSLSocketFactory
			' at construction time - because the RMI client socket factory is
			' created on the server side, where that initialization is a priori
			' meaningless, unless both server and client run in the same JVM.
			' We could possibly override readObject() to force this initialization,
			' but it might not be a good idea to actually mix this with possible
			' deserialization problems.
			' So contrarily to what we do for the server side, the initialization
			' of the SSLSocketFactory will be delayed until the first time
			' createSocket() is called - note that the default SSLSocketFactory
			' might already have been initialized anyway if someone in the JVM
			' already called SSLSocketFactory.getDefault().
			'
		End Sub

		''' <summary>
		''' <p>Creates an SSL socket.</p>
		''' 
		''' <p>If the system property
		''' <code>javax.rmi.ssl.client.enabledCipherSuites</code> is
		''' specified, this method will call {@link
		''' SSLSocket#setEnabledCipherSuites(String[])} before returning
		''' the socket. The value of this system property is a string that
		''' is a comma-separated list of SSL/TLS cipher suites to
		''' enable.</p>
		''' 
		''' <p>If the system property
		''' <code>javax.rmi.ssl.client.enabledProtocols</code> is
		''' specified, this method will call {@link
		''' SSLSocket#setEnabledProtocols(String[])} before returning the
		''' socket. The value of this system property is a string that is a
		''' comma-separated list of SSL/TLS protocol versions to
		''' enable.</p>
		''' </summary>
		Public Overridable Function createSocket(ByVal host As String, ByVal port As Integer) As java.net.Socket
			' Retrieve the SSLSocketFactory
			'
			Dim sslSocketFactory As javax.net.SocketFactory = defaultClientSocketFactory
			' Create the SSLSocket
			'
			Dim sslSocket As javax.net.ssl.SSLSocket = CType(sslSocketFactory.createSocket(host, port), javax.net.ssl.SSLSocket)
			' Set the SSLSocket Enabled Cipher Suites
			'
			Dim enabledCipherSuites As String = System.getProperty("javax.rmi.ssl.client.enabledCipherSuites")
			If enabledCipherSuites IsNot Nothing Then
				Dim st As New java.util.StringTokenizer(enabledCipherSuites, ",")
				Dim tokens As Integer = st.countTokens()
				Dim enabledCipherSuitesList As String() = New String(tokens - 1){}
				For i As Integer = 0 To tokens - 1
					enabledCipherSuitesList(i) = st.nextToken()
				Next i
				Try
					sslSocket.enabledCipherSuites = enabledCipherSuitesList
				Catch e As System.ArgumentException
					Throw CType((New java.io.IOException(e.Message)).initCause(e), java.io.IOException)
				End Try
			End If
			' Set the SSLSocket Enabled Protocols
			'
			Dim enabledProtocols As String = System.getProperty("javax.rmi.ssl.client.enabledProtocols")
			If enabledProtocols IsNot Nothing Then
				Dim st As New java.util.StringTokenizer(enabledProtocols, ",")
				Dim tokens As Integer = st.countTokens()
				Dim enabledProtocolsList As String() = New String(tokens - 1){}
				For i As Integer = 0 To tokens - 1
					enabledProtocolsList(i) = st.nextToken()
				Next i
				Try
					sslSocket.enabledProtocols = enabledProtocolsList
				Catch e As System.ArgumentException
					Throw CType((New java.io.IOException(e.Message)).initCause(e), java.io.IOException)
				End Try
			End If
			' Return the preconfigured SSLSocket
			'
			Return sslSocket
		End Function

		''' <summary>
		''' <p>Indicates whether some other object is "equal to" this one.</p>
		''' 
		''' <p>Because all instances of this class are functionally equivalent
		''' (they all use the default
		''' <code>SSLSocketFactory</code>), this method simply returns
		''' <code>this.getClass().equals(obj.getClass())</code>.</p>
		''' 
		''' <p>A subclass should override this method (as well
		''' as <seealso cref="#hashCode()"/>) if its instances are not all
		''' functionally equivalent.</p>
		''' </summary>
		Public Overrides Function Equals(ByVal obj As Object) As Boolean
			If obj Is Nothing Then Return False
			If obj Is Me Then Return True
			Return Me.GetType().Equals(obj.GetType())
		End Function

		''' <summary>
		''' <p>Returns a hash code value for this
		''' <code>SslRMIClientSocketFactory</code>.</p>
		''' </summary>
		''' <returns> a hash code value for this
		''' <code>SslRMIClientSocketFactory</code>. </returns>
		Public Overrides Function GetHashCode() As Integer
			Return Me.GetType().GetHashCode()
		End Function

		' We use a static field because:
		'
		'    SSLSocketFactory.getDefault() always returns the same object
		'    (at least on Sun's implementation), and we want to make sure
		'    that the Javadoc & the implementation stay in sync.
		'
		' If someone needs to have different SslRMIClientSocketFactory factories
		' with different underlying SSLSocketFactory objects using different key
		' and trust stores, he can always do so by subclassing this class and
		' overriding createSocket(String host, int port).
		'
		Private Shared defaultSocketFactory As javax.net.SocketFactory = Nothing

		<MethodImpl(MethodImplOptions.Synchronized)> _
		Private Property Shared defaultClientSocketFactory As javax.net.SocketFactory
			Get
				If defaultSocketFactory Is Nothing Then defaultSocketFactory = javax.net.ssl.SSLSocketFactory.default
				Return defaultSocketFactory
			End Get
		End Property

		Private Const serialVersionUID As Long = -8310631444933958385L
	End Class

End Namespace