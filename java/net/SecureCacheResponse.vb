Imports System.Collections.Generic

'
' * Copyright (c) 2003, 2004, Oracle and/or its affiliates. All rights reserved.
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
	''' Represents a cache response originally retrieved through secure
	''' means, such as TLS.
	''' 
	''' @since 1.5
	''' </summary>
	Public MustInherit Class SecureCacheResponse
		Inherits CacheResponse

		''' <summary>
		''' Returns the cipher suite in use on the original connection that
		''' retrieved the network resource.
		''' </summary>
		''' <returns> a string representing the cipher suite </returns>
		Public MustOverride ReadOnly Property cipherSuite As String

		''' <summary>
		''' Returns the certificate chain that were sent to the server during
		''' handshaking of the original connection that retrieved the
		''' network resource.  Note: This method is useful only
		''' when using certificate-based cipher suites.
		''' </summary>
		''' <returns> an immutable List of Certificate representing the
		'''           certificate chain that was sent to the server. If no
		'''           certificate chain was sent, null will be returned. </returns>
		''' <seealso cref= #getLocalPrincipal() </seealso>
		Public MustOverride ReadOnly Property localCertificateChain As IList(Of java.security.cert.Certificate)

		''' <summary>
		''' Returns the server's certificate chain, which was established as
		''' part of defining the session in the original connection that
		''' retrieved the network resource, from cache.  Note: This method
		''' can be used only when using certificate-based cipher suites;
		''' using it with non-certificate-based cipher suites, such as
		''' Kerberos, will throw an SSLPeerUnverifiedException.
		''' </summary>
		''' <returns> an immutable List of Certificate representing the server's
		'''         certificate chain. </returns>
		''' <exception cref="SSLPeerUnverifiedException"> if the peer is not verified. </exception>
		''' <seealso cref= #getPeerPrincipal() </seealso>
		Public MustOverride ReadOnly Property serverCertificateChain As IList(Of java.security.cert.Certificate)

		''' <summary>
		''' Returns the server's principal which was established as part of
		''' defining the session during the original connection that
		''' retrieved the network resource.
		''' </summary>
		''' <returns> the server's principal. Returns an X500Principal of the
		''' end-entity certiticate for X509-based cipher suites, and
		''' KerberosPrincipal for Kerberos cipher suites.
		''' </returns>
		''' <exception cref="SSLPeerUnverifiedException"> if the peer was not verified.
		''' </exception>
		''' <seealso cref= #getServerCertificateChain() </seealso>
		''' <seealso cref= #getLocalPrincipal() </seealso>
		 Public MustOverride ReadOnly Property peerPrincipal As java.security.Principal

		''' <summary>
		''' Returns the principal that was sent to the server during
		''' handshaking in the original connection that retrieved the
		''' network resource.
		''' </summary>
		''' <returns> the principal sent to the server. Returns an X500Principal
		''' of the end-entity certificate for X509-based cipher suites, and
		''' KerberosPrincipal for Kerberos cipher suites. If no principal was
		''' sent, then null is returned.
		''' </returns>
		''' <seealso cref= #getLocalCertificateChain() </seealso>
		''' <seealso cref= #getPeerPrincipal() </seealso>
		 Public MustOverride ReadOnly Property localPrincipal As java.security.Principal
	End Class

End Namespace