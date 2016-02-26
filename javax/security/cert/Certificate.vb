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


Namespace javax.security.cert


	''' <summary>
	''' <p>Abstract class for managing a variety of identity certificates.
	''' An identity certificate is a guarantee by a principal that
	''' a public key is that of another principal.  (A principal represents
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
	''' 
	''' <p><em>Note: The classes in the package {@code javax.security.cert}
	''' exist for compatibility with earlier versions of the
	''' Java Secure Sockets Extension (JSSE). New applications should instead
	''' use the standard Java SE certificate classes located in
	''' {@code java.security.cert}.</em></p>
	''' 
	''' @since 1.4 </summary>
	''' <seealso cref= X509Certificate
	''' 
	''' @author Hemma Prafullchandra </seealso>
	Public MustInherit Class Certificate

		''' <summary>
		''' Compares this certificate for equality with the specified
		''' object. If the {@code other} object is an
		''' {@code instanceof} {@code Certificate}, then
		''' its encoded form is retrieved and compared with the
		''' encoded form of this certificate.
		''' </summary>
		''' <param name="other"> the object to test for equality with this certificate. </param>
		''' <returns> true if the encoded forms of the two certificates
		'''         match, false otherwise. </returns>
		Public Overrides Function Equals(ByVal other As Object) As Boolean
			If Me Is other Then Return True
			If Not(TypeOf other Is Certificate) Then Return False
			Try
				Dim thisCert As SByte() = Me.encoded
				Dim otherCert As SByte() = CType(other, Certificate).encoded

				If thisCert.Length <> otherCert.Length Then Return False
				For i As Integer = 0 To thisCert.Length - 1
					 If thisCert(i) <> otherCert(i) Then Return False
				Next i
				Return True
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
			Dim retval As Integer = 0
			Try
				Dim certData As SByte() = Me.encoded
				For i As Integer = 1 To certData.Length - 1
					 retval += certData(i) * i
				Next i
				Return (retval)
			Catch e As CertificateException
				Return (retval)
			End Try
		End Function

		''' <summary>
		''' Returns the encoded form of this certificate. It is
		''' assumed that each certificate type would have only a single
		''' form of encoding; for example, X.509 certificates would
		''' be encoded as ASN.1 DER.
		''' </summary>
		''' <returns> encoded form of this certificate </returns>
		''' <exception cref="CertificateEncodingException"> on internal certificate
		'''            encoding failure </exception>
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
		''' <param name="sigProvider"> the name of the signature provider. </param>
		''' <exception cref="NoSuchAlgorithmException"> on unsupported signature algorithms. </exception>
		''' <exception cref="InvalidKeyException"> on incorrect key. </exception>
		''' <exception cref="NoSuchProviderException"> on incorrect provider. </exception>
		''' <exception cref="SignatureException"> on signature errors. </exception>
		''' <exception cref="CertificateException"> on encoding errors. </exception>
		Public MustOverride Sub verify(ByVal key As java.security.PublicKey, ByVal sigProvider As String)

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
	End Class

End Namespace