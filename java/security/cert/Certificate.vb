Imports System

'
' * Copyright (c) 1997, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.security.cert




	''' <summary>
	''' <p>Abstract class for managing a variety of identity certificates.
	''' An identity certificate is a binding of a principal to a public key which
	''' is vouched for by another principal.  (A principal represents
	''' an entity such as an individual user, a group, or a corporation.)
	''' <p>
	''' This class is an abstraction for certificates that have different
	''' formats but important common uses.  For example, different types of
	''' certificates, such as X.509 and PGP, share general certificate
	''' functionality (like encoding and verifying) and
	''' some types of information (like a public key).
	''' <p>
	''' X.509, PGP, and SDSI certificates can all be implemented by
	''' subclassing the Certificate class, even though they contain different
	''' sets of information, and they store and retrieve the information in
	''' different ways.
	''' </summary>
	''' <seealso cref= X509Certificate </seealso>
	''' <seealso cref= CertificateFactory
	''' 
	''' @author Hemma Prafullchandra </seealso>

	<Serializable> _
	Public MustInherit Class Certificate

		Private Const serialVersionUID As Long = -3585440601605666277L

		' the certificate type
		Private ReadOnly type As String

		''' <summary>
		''' Cache the hash code for the certiticate </summary>
		Private hash As Integer = -1 ' Default to -1

		''' <summary>
		''' Creates a certificate of the specified type.
		''' </summary>
		''' <param name="type"> the standard name of the certificate type.
		''' See the CertificateFactory section in the <a href=
		''' "{@docRoot}/../technotes/guides/security/StandardNames.html#CertificateFactory">
		''' Java Cryptography Architecture Standard Algorithm Name Documentation</a>
		''' for information about standard certificate types. </param>
		Protected Friend Sub New(ByVal type As String)
			Me.type = type
		End Sub

		''' <summary>
		''' Returns the type of this certificate.
		''' </summary>
		''' <returns> the type of this certificate. </returns>
		Public Property type As String
			Get
				Return Me.type
			End Get
		End Property

		''' <summary>
		''' Compares this certificate for equality with the specified
		''' object. If the {@code other} object is an
		''' {@code instanceof} {@code Certificate}, then
		''' its encoded form is retrieved and compared with the
		''' encoded form of this certificate.
		''' </summary>
		''' <param name="other"> the object to test for equality with this certificate. </param>
		''' <returns> true iff the encoded forms of the two certificates
		''' match, false otherwise. </returns>
		Public Overrides Function Equals(ByVal other As Object) As Boolean
			If Me Is other Then Return True
			If Not(TypeOf other Is Certificate) Then Return False
			Try
				Dim thisCert As SByte() = sun.security.x509.X509CertImpl.getEncodedInternal(Me)
				Dim otherCert As SByte() = sun.security.x509.X509CertImpl.getEncodedInternal(CType(other, Certificate))

				Return java.util.Arrays.Equals(thisCert, otherCert)
			Catch e As CertificateException
				Return False
			End Try
		End Function

		''' <summary>
		''' Returns a hashcode value for this certificate from its
		''' encoded form.
		''' </summary>
		''' <returns> the hashcode value. </returns>
		Public Overrides Function GetHashCode() As Integer
			Dim h As Integer = hash
			If h = -1 Then
				Try
					h = java.util.Arrays.hashCode(sun.security.x509.X509CertImpl.getEncodedInternal(Me))
				Catch e As CertificateException
					h = 0
				End Try
				hash = h
			End If
			Return h
		End Function

		''' <summary>
		''' Returns the encoded form of this certificate. It is
		''' assumed that each certificate type would have only a single
		''' form of encoding; for example, X.509 certificates would
		''' be encoded as ASN.1 DER.
		''' </summary>
		''' <returns> the encoded form of this certificate
		''' </returns>
		''' <exception cref="CertificateEncodingException"> if an encoding error occurs. </exception>
		Public MustOverride ReadOnly Property encoded As SByte()

		''' <summary>
		''' Verifies that this certificate was signed using the
		''' private key that corresponds to the specified public key.
		''' </summary>
		''' <param name="key"> the PublicKey used to carry out the verification.
		''' </param>
		''' <exception cref="NoSuchAlgorithmException"> on unsupported signature
		''' algorithms. </exception>
		''' <exception cref="InvalidKeyException"> on incorrect key. </exception>
		''' <exception cref="NoSuchProviderException"> if there's no default provider. </exception>
		''' <exception cref="SignatureException"> on signature errors. </exception>
		''' <exception cref="CertificateException"> on encoding errors. </exception>
		Public MustOverride Sub verify(ByVal key As java.security.PublicKey)

		''' <summary>
		''' Verifies that this certificate was signed using the
		''' private key that corresponds to the specified public key.
		''' This method uses the signature verification engine
		''' supplied by the specified provider.
		''' </summary>
		''' <param name="key"> the PublicKey used to carry out the verification. </param>
		''' <param name="sigProvider"> the name of the signature provider.
		''' </param>
		''' <exception cref="NoSuchAlgorithmException"> on unsupported signature
		''' algorithms. </exception>
		''' <exception cref="InvalidKeyException"> on incorrect key. </exception>
		''' <exception cref="NoSuchProviderException"> on incorrect provider. </exception>
		''' <exception cref="SignatureException"> on signature errors. </exception>
		''' <exception cref="CertificateException"> on encoding errors. </exception>
		Public MustOverride Sub verify(ByVal key As java.security.PublicKey, ByVal sigProvider As String)

		''' <summary>
		''' Verifies that this certificate was signed using the
		''' private key that corresponds to the specified public key.
		''' This method uses the signature verification engine
		''' supplied by the specified provider. Note that the specified
		''' Provider object does not have to be registered in the provider list.
		''' 
		''' <p> This method was added to version 1.8 of the Java Platform
		''' Standard Edition. In order to maintain backwards compatibility with
		''' existing service providers, this method cannot be {@code abstract}
		''' and by default throws an {@code UnsupportedOperationException}.
		''' </summary>
		''' <param name="key"> the PublicKey used to carry out the verification. </param>
		''' <param name="sigProvider"> the signature provider.
		''' </param>
		''' <exception cref="NoSuchAlgorithmException"> on unsupported signature
		''' algorithms. </exception>
		''' <exception cref="InvalidKeyException"> on incorrect key. </exception>
		''' <exception cref="SignatureException"> on signature errors. </exception>
		''' <exception cref="CertificateException"> on encoding errors. </exception>
		''' <exception cref="UnsupportedOperationException"> if the method is not supported
		''' @since 1.8 </exception>
		Public Overridable Sub verify(ByVal key As java.security.PublicKey, ByVal sigProvider As java.security.Provider)
			Throw New UnsupportedOperationException
		End Sub

		''' <summary>
		''' Returns a string representation of this certificate.
		''' </summary>
		''' <returns> a string representation of this certificate. </returns>
		Public MustOverride Function ToString() As String

		''' <summary>
		''' Gets the public key from this certificate.
		''' </summary>
		''' <returns> the public key. </returns>
		Public MustOverride ReadOnly Property publicKey As java.security.PublicKey

		''' <summary>
		''' Alternate Certificate class for serialization.
		''' @since 1.3
		''' </summary>
		<Serializable> _
		Protected Friend Class CertificateRep

			Private Const serialVersionUID As Long = -8563758940495660020L

			Private type As String
			Private data As SByte()

			''' <summary>
			''' Construct the alternate Certificate class with the Certificate
			''' type and Certificate encoding bytes.
			''' 
			''' <p>
			''' </summary>
			''' <param name="type"> the standard name of the Certificate type. <p>
			''' </param>
			''' <param name="data"> the Certificate data. </param>
			Protected Friend Sub New(ByVal type As String, ByVal data As SByte())
				Me.type = type
				Me.data = data
			End Sub

			''' <summary>
			''' Resolve the Certificate Object.
			''' 
			''' <p>
			''' </summary>
			''' <returns> the resolved Certificate Object
			''' </returns>
			''' <exception cref="java.io.ObjectStreamException"> if the Certificate
			'''      could not be resolved </exception>
			Protected Friend Overridable Function readResolve() As Object
				Try
					Dim cf As CertificateFactory = CertificateFactory.getInstance(type)
					Return cf.generateCertificate(New java.io.ByteArrayInputStream(data))
				Catch e As CertificateException
					Throw New java.io.NotSerializableException("java.security.cert.Certificate: " & type & ": " & e.Message)
				End Try
			End Function
		End Class

		''' <summary>
		''' Replace the Certificate to be serialized.
		''' </summary>
		''' <returns> the alternate Certificate object to be serialized
		''' </returns>
		''' <exception cref="java.io.ObjectStreamException"> if a new object representing
		''' this Certificate could not be created
		''' @since 1.3 </exception>
		Protected Friend Overridable Function writeReplace() As Object
			Try
				Return New CertificateRep(type, encoded)
			Catch e As CertificateException
				Throw New java.io.NotSerializableException("java.security.cert.Certificate: " & type & ": " & e.Message)
			End Try
		End Function
	End Class

End Namespace