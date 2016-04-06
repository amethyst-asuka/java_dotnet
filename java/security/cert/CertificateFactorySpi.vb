Imports System.Collections.Generic

'
' * Copyright (c) 1998, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' This class defines the <i>Service Provider Interface</i> (<b>SPI</b>)
	''' for the {@code CertificateFactory} class.
	''' All the abstract methods in this class must be implemented by each
	''' cryptographic service provider who wishes to supply the implementation
	''' of a certificate factory for a particular certificate type, e.g., X.509.
	''' 
	''' <p>Certificate factories are used to generate certificate, certification path
	''' ({@code CertPath}) and certificate revocation list (CRL) objects from
	''' their encodings.
	''' 
	''' <p>A certificate factory for X.509 must return certificates that are an
	''' instance of {@code java.security.cert.X509Certificate}, and CRLs
	''' that are an instance of {@code java.security.cert.X509CRL}.
	''' 
	''' @author Hemma Prafullchandra
	''' @author Jan Luehe
	''' @author Sean Mullan
	''' 
	''' </summary>
	''' <seealso cref= CertificateFactory </seealso>
	''' <seealso cref= Certificate </seealso>
	''' <seealso cref= X509Certificate </seealso>
	''' <seealso cref= CertPath </seealso>
	''' <seealso cref= CRL </seealso>
	''' <seealso cref= X509CRL
	''' 
	''' @since 1.2 </seealso>

	Public MustInherit Class CertificateFactorySpi

		''' <summary>
		''' Generates a certificate object and initializes it with
		''' the data read from the input stream {@code inStream}.
		''' 
		''' <p>In order to take advantage of the specialized certificate format
		''' supported by this certificate factory,
		''' the returned certificate object can be typecast to the corresponding
		''' certificate class. For example, if this certificate
		''' factory implements X.509 certificates, the returned certificate object
		''' can be typecast to the {@code X509Certificate} class.
		''' 
		''' <p>In the case of a certificate factory for X.509 certificates, the
		''' certificate provided in {@code inStream} must be DER-encoded and
		''' may be supplied in binary or printable (Base64) encoding. If the
		''' certificate is provided in Base64 encoding, it must be bounded at
		''' the beginning by -----BEGIN CERTIFICATE-----, and must be bounded at
		''' the end by -----END CERTIFICATE-----.
		''' 
		''' <p>Note that if the given input stream does not support
		''' <seealso cref="java.io.InputStream#mark(int) mark"/> and
		''' <seealso cref="java.io.InputStream#reset() reset"/>, this method will
		''' consume the entire input stream. Otherwise, each call to this
		''' method consumes one certificate and the read position of the input stream
		''' is positioned to the next available byte after the inherent
		''' end-of-certificate marker. If the data in the
		''' input stream does not contain an inherent end-of-certificate marker (other
		''' than EOF) and there is trailing data after the certificate is parsed, a
		''' {@code CertificateException} is thrown.
		''' </summary>
		''' <param name="inStream"> an input stream with the certificate data.
		''' </param>
		''' <returns> a certificate object initialized with the data
		''' from the input stream.
		''' </returns>
		''' <exception cref="CertificateException"> on parsing errors. </exception>
		Public MustOverride Function engineGenerateCertificate(  inStream As java.io.InputStream) As Certificate

		''' <summary>
		''' Generates a {@code CertPath} object and initializes it with
		''' the data read from the {@code InputStream} inStream. The data
		''' is assumed to be in the default encoding.
		''' 
		''' <p> This method was added to version 1.4 of the Java 2 Platform
		''' Standard Edition. In order to maintain backwards compatibility with
		''' existing service providers, this method cannot be {@code abstract}
		''' and by default throws an {@code UnsupportedOperationException}.
		''' </summary>
		''' <param name="inStream"> an {@code InputStream} containing the data </param>
		''' <returns> a {@code CertPath} initialized with the data from the
		'''   {@code InputStream} </returns>
		''' <exception cref="CertificateException"> if an exception occurs while decoding </exception>
		''' <exception cref="UnsupportedOperationException"> if the method is not supported
		''' @since 1.4 </exception>
		Public Overridable Function engineGenerateCertPath(  inStream As java.io.InputStream) As CertPath
			Throw New UnsupportedOperationException
		End Function

		''' <summary>
		''' Generates a {@code CertPath} object and initializes it with
		''' the data read from the {@code InputStream} inStream. The data
		''' is assumed to be in the specified encoding.
		''' 
		''' <p> This method was added to version 1.4 of the Java 2 Platform
		''' Standard Edition. In order to maintain backwards compatibility with
		''' existing service providers, this method cannot be {@code abstract}
		''' and by default throws an {@code UnsupportedOperationException}.
		''' </summary>
		''' <param name="inStream"> an {@code InputStream} containing the data </param>
		''' <param name="encoding"> the encoding used for the data </param>
		''' <returns> a {@code CertPath} initialized with the data from the
		'''   {@code InputStream} </returns>
		''' <exception cref="CertificateException"> if an exception occurs while decoding or
		'''   the encoding requested is not supported </exception>
		''' <exception cref="UnsupportedOperationException"> if the method is not supported
		''' @since 1.4 </exception>
		Public Overridable Function engineGenerateCertPath(  inStream As java.io.InputStream,   encoding As String) As CertPath
			Throw New UnsupportedOperationException
		End Function

		''' <summary>
		''' Generates a {@code CertPath} object and initializes it with
		''' a {@code List} of {@code Certificate}s.
		''' <p>
		''' The certificates supplied must be of a type supported by the
		''' {@code CertificateFactory}. They will be copied out of the supplied
		''' {@code List} object.
		''' 
		''' <p> This method was added to version 1.4 of the Java 2 Platform
		''' Standard Edition. In order to maintain backwards compatibility with
		''' existing service providers, this method cannot be {@code abstract}
		''' and by default throws an {@code UnsupportedOperationException}.
		''' </summary>
		''' <param name="certificates"> a {@code List} of {@code Certificate}s </param>
		''' <returns> a {@code CertPath} initialized with the supplied list of
		'''   certificates </returns>
		''' <exception cref="CertificateException"> if an exception occurs </exception>
		''' <exception cref="UnsupportedOperationException"> if the method is not supported
		''' @since 1.4 </exception>
		Public Overridable Function engineGenerateCertPath(Of T1 As Certificate)(  certificates As IList(Of T1)) As CertPath
			Throw New UnsupportedOperationException
		End Function

		''' <summary>
		''' Returns an iteration of the {@code CertPath} encodings supported
		''' by this certificate factory, with the default encoding first. See
		''' the CertPath Encodings section in the <a href=
		''' "{@docRoot}/../technotes/guides/security/StandardNames.html#CertPathEncodings">
		''' Java Cryptography Architecture Standard Algorithm Name Documentation</a>
		''' for information about standard encoding names.
		''' <p>
		''' Attempts to modify the returned {@code Iterator} via its
		''' {@code remove} method result in an
		''' {@code UnsupportedOperationException}.
		''' 
		''' <p> This method was added to version 1.4 of the Java 2 Platform
		''' Standard Edition. In order to maintain backwards compatibility with
		''' existing service providers, this method cannot be {@code abstract}
		''' and by default throws an {@code UnsupportedOperationException}.
		''' </summary>
		''' <returns> an {@code Iterator} over the names of the supported
		'''         {@code CertPath} encodings (as {@code String}s) </returns>
		''' <exception cref="UnsupportedOperationException"> if the method is not supported
		''' @since 1.4 </exception>
		Public Overridable Function engineGetCertPathEncodings() As IEnumerator(Of String)
			Throw New UnsupportedOperationException
		End Function

		''' <summary>
		''' Returns a (possibly empty) collection view of the certificates read
		''' from the given input stream {@code inStream}.
		''' 
		''' <p>In order to take advantage of the specialized certificate format
		''' supported by this certificate factory, each element in
		''' the returned collection view can be typecast to the corresponding
		''' certificate class. For example, if this certificate
		''' factory implements X.509 certificates, the elements in the returned
		''' collection can be typecast to the {@code X509Certificate} class.
		''' 
		''' <p>In the case of a certificate factory for X.509 certificates,
		''' {@code inStream} may contain a single DER-encoded certificate
		''' in the formats described for
		''' {@link CertificateFactory#generateCertificate(java.io.InputStream)
		''' generateCertificate}.
		''' In addition, {@code inStream} may contain a PKCS#7 certificate
		''' chain. This is a PKCS#7 <i>SignedData</i> object, with the only
		''' significant field being <i>certificates</i>. In particular, the
		''' signature and the contents are ignored. This format allows multiple
		''' certificates to be downloaded at once. If no certificates are present,
		''' an empty collection is returned.
		''' 
		''' <p>Note that if the given input stream does not support
		''' <seealso cref="java.io.InputStream#mark(int) mark"/> and
		''' <seealso cref="java.io.InputStream#reset() reset"/>, this method will
		''' consume the entire input stream.
		''' </summary>
		''' <param name="inStream"> the input stream with the certificates.
		''' </param>
		''' <returns> a (possibly empty) collection view of
		''' java.security.cert.Certificate objects
		''' initialized with the data from the input stream.
		''' </returns>
		''' <exception cref="CertificateException"> on parsing errors. </exception>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Public MustOverride Function engineGenerateCertificates(  inStream As java.io.InputStream) As ICollection(Of ? As Certificate)

		''' <summary>
		''' Generates a certificate revocation list (CRL) object and initializes it
		''' with the data read from the input stream {@code inStream}.
		''' 
		''' <p>In order to take advantage of the specialized CRL format
		''' supported by this certificate factory,
		''' the returned CRL object can be typecast to the corresponding
		''' CRL class. For example, if this certificate
		''' factory implements X.509 CRLs, the returned CRL object
		''' can be typecast to the {@code X509CRL} class.
		''' 
		''' <p>Note that if the given input stream does not support
		''' <seealso cref="java.io.InputStream#mark(int) mark"/> and
		''' <seealso cref="java.io.InputStream#reset() reset"/>, this method will
		''' consume the entire input stream. Otherwise, each call to this
		''' method consumes one CRL and the read position of the input stream
		''' is positioned to the next available byte after the inherent
		''' end-of-CRL marker. If the data in the
		''' input stream does not contain an inherent end-of-CRL marker (other
		''' than EOF) and there is trailing data after the CRL is parsed, a
		''' {@code CRLException} is thrown.
		''' </summary>
		''' <param name="inStream"> an input stream with the CRL data.
		''' </param>
		''' <returns> a CRL object initialized with the data
		''' from the input stream.
		''' </returns>
		''' <exception cref="CRLException"> on parsing errors. </exception>
		Public MustOverride Function engineGenerateCRL(  inStream As java.io.InputStream) As CRL

		''' <summary>
		''' Returns a (possibly empty) collection view of the CRLs read
		''' from the given input stream {@code inStream}.
		''' 
		''' <p>In order to take advantage of the specialized CRL format
		''' supported by this certificate factory, each element in
		''' the returned collection view can be typecast to the corresponding
		''' CRL class. For example, if this certificate
		''' factory implements X.509 CRLs, the elements in the returned
		''' collection can be typecast to the {@code X509CRL} class.
		''' 
		''' <p>In the case of a certificate factory for X.509 CRLs,
		''' {@code inStream} may contain a single DER-encoded CRL.
		''' In addition, {@code inStream} may contain a PKCS#7 CRL
		''' set. This is a PKCS#7 <i>SignedData</i> object, with the only
		''' significant field being <i>crls</i>. In particular, the
		''' signature and the contents are ignored. This format allows multiple
		''' CRLs to be downloaded at once. If no CRLs are present,
		''' an empty collection is returned.
		''' 
		''' <p>Note that if the given input stream does not support
		''' <seealso cref="java.io.InputStream#mark(int) mark"/> and
		''' <seealso cref="java.io.InputStream#reset() reset"/>, this method will
		''' consume the entire input stream.
		''' </summary>
		''' <param name="inStream"> the input stream with the CRLs.
		''' </param>
		''' <returns> a (possibly empty) collection view of
		''' java.security.cert.CRL objects initialized with the data from the input
		''' stream.
		''' </returns>
		''' <exception cref="CRLException"> on parsing errors. </exception>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Public MustOverride Function engineGenerateCRLs(  inStream As java.io.InputStream) As ICollection(Of ? As CRL)
	End Class

End Namespace