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
	''' <p>
	''' Abstract class for an X.509 Certificate Revocation List (CRL).
	''' A CRL is a time-stamped list identifying revoked certificates.
	''' It is signed by a Certificate Authority (CA) and made freely
	''' available in a public repository.
	''' 
	''' <p>Each revoked certificate is
	''' identified in a CRL by its certificate serial number. When a
	''' certificate-using system uses a certificate (e.g., for verifying a
	''' remote user's digital signature), that system not only checks the
	''' certificate signature and validity but also acquires a suitably-
	''' recent CRL and checks that the certificate serial number is not on
	''' that CRL.  The meaning of "suitably-recent" may vary with local
	''' policy, but it usually means the most recently-issued CRL.  A CA
	''' issues a new CRL on a regular periodic basis (e.g., hourly, daily, or
	''' weekly).  Entries are added to CRLs as revocations occur, and an
	''' entry may be removed when the certificate expiration date is reached.
	''' <p>
	''' The X.509 v2 CRL format is described below in ASN.1:
	''' <pre>
	''' CertificateList  ::=  SEQUENCE  {
	'''     tbsCertList          TBSCertList,
	'''     signatureAlgorithm   AlgorithmIdentifier,
	'''     signature            BIT STRING  }
	''' </pre>
	''' <p>
	''' More information can be found in
	''' <a href="http://www.ietf.org/rfc/rfc3280.txt">RFC 3280: Internet X.509
	''' Public Key Infrastructure Certificate and CRL Profile</a>.
	''' <p>
	''' The ASN.1 definition of {@code tbsCertList} is:
	''' <pre>
	''' TBSCertList  ::=  SEQUENCE  {
	'''     version                 Version OPTIONAL,
	'''                             -- if present, must be v2
	'''     signature               AlgorithmIdentifier,
	'''     issuer                  Name,
	'''     thisUpdate              ChoiceOfTime,
	'''     nextUpdate              ChoiceOfTime OPTIONAL,
	'''     revokedCertificates     SEQUENCE OF SEQUENCE  {
	'''         userCertificate         CertificateSerialNumber,
	'''         revocationDate          ChoiceOfTime,
	'''         crlEntryExtensions      Extensions OPTIONAL
	'''                                 -- if present, must be v2
	'''         }  OPTIONAL,
	'''     crlExtensions           [0]  EXPLICIT Extensions OPTIONAL
	'''                                  -- if present, must be v2
	'''     }
	''' </pre>
	''' <p>
	''' CRLs are instantiated using a certificate factory. The following is an
	''' example of how to instantiate an X.509 CRL:
	''' <pre>{@code
	''' try (InputStream inStream = new FileInputStream("fileName-of-crl")) {
	'''     CertificateFactory cf = CertificateFactory.getInstance("X.509");
	'''     X509CRL crl = (X509CRL)cf.generateCRL(inStream);
	''' }
	''' }</pre>
	''' 
	''' @author Hemma Prafullchandra
	''' 
	''' </summary>
	''' <seealso cref= CRL </seealso>
	''' <seealso cref= CertificateFactory </seealso>
	''' <seealso cref= X509Extension </seealso>

	Public MustInherit Class X509CRL
		Inherits CRL
		Implements X509Extension

			Public MustOverride Function getExtensionValue(ByVal oid As String) As SByte() Implements X509Extension.getExtensionValue
			Public MustOverride ReadOnly Property nonCriticalExtensionOIDs As java.util.Set(Of String) Implements X509Extension.getNonCriticalExtensionOIDs
			Public MustOverride ReadOnly Property criticalExtensionOIDs As java.util.Set(Of String) Implements X509Extension.getCriticalExtensionOIDs
			Public MustOverride Function hasUnsupportedCriticalExtension() As Boolean Implements X509Extension.hasUnsupportedCriticalExtension

		<NonSerialized> _
		Private issuerPrincipal As javax.security.auth.x500.X500Principal

		''' <summary>
		''' Constructor for X.509 CRLs.
		''' </summary>
		Protected Friend Sub New()
			MyBase.New("X.509")
		End Sub

		''' <summary>
		''' Compares this CRL for equality with the given
		''' object. If the {@code other} object is an
		''' {@code instanceof} {@code X509CRL}, then
		''' its encoded form is retrieved and compared with the
		''' encoded form of this CRL.
		''' </summary>
		''' <param name="other"> the object to test for equality with this CRL.
		''' </param>
		''' <returns> true iff the encoded forms of the two CRLs
		''' match, false otherwise. </returns>
		Public Overrides Function Equals(ByVal other As Object) As Boolean
			If Me Is other Then Return True
			If Not(TypeOf other Is X509CRL) Then Return False
			Try
				Dim thisCRL As SByte() = sun.security.x509.X509CRLImpl.getEncodedInternal(Me)
				Dim otherCRL As SByte() = sun.security.x509.X509CRLImpl.getEncodedInternal(CType(other, X509CRL))

				Return java.util.Arrays.Equals(thisCRL, otherCRL)
			Catch e As CRLException
				Return False
			End Try
		End Function

		''' <summary>
		''' Returns a hashcode value for this CRL from its
		''' encoded form.
		''' </summary>
		''' <returns> the hashcode value. </returns>
		Public Overrides Function GetHashCode() As Integer
			Dim retval As Integer = 0
			Try
				Dim crlData As SByte() = sun.security.x509.X509CRLImpl.getEncodedInternal(Me)
				For i As Integer = 1 To crlData.Length - 1
					 retval += crlData(i) * i
				Next i
				Return retval
			Catch e As CRLException
				Return retval
			End Try
		End Function

		''' <summary>
		''' Returns the ASN.1 DER-encoded form of this CRL.
		''' </summary>
		''' <returns> the encoded form of this certificate </returns>
		''' <exception cref="CRLException"> if an encoding error occurs. </exception>
		Public MustOverride ReadOnly Property encoded As SByte()

		''' <summary>
		''' Verifies that this CRL was signed using the
		''' private key that corresponds to the given public key.
		''' </summary>
		''' <param name="key"> the PublicKey used to carry out the verification.
		''' </param>
		''' <exception cref="NoSuchAlgorithmException"> on unsupported signature
		''' algorithms. </exception>
		''' <exception cref="InvalidKeyException"> on incorrect key. </exception>
		''' <exception cref="NoSuchProviderException"> if there's no default provider. </exception>
		''' <exception cref="SignatureException"> on signature errors. </exception>
		''' <exception cref="CRLException"> on encoding errors. </exception>
		Public MustOverride Sub verify(ByVal key As java.security.PublicKey)

		''' <summary>
		''' Verifies that this CRL was signed using the
		''' private key that corresponds to the given public key.
		''' This method uses the signature verification engine
		''' supplied by the given provider.
		''' </summary>
		''' <param name="key"> the PublicKey used to carry out the verification. </param>
		''' <param name="sigProvider"> the name of the signature provider.
		''' </param>
		''' <exception cref="NoSuchAlgorithmException"> on unsupported signature
		''' algorithms. </exception>
		''' <exception cref="InvalidKeyException"> on incorrect key. </exception>
		''' <exception cref="NoSuchProviderException"> on incorrect provider. </exception>
		''' <exception cref="SignatureException"> on signature errors. </exception>
		''' <exception cref="CRLException"> on encoding errors. </exception>
		Public MustOverride Sub verify(ByVal key As java.security.PublicKey, ByVal sigProvider As String)

		''' <summary>
		''' Verifies that this CRL was signed using the
		''' private key that corresponds to the given public key.
		''' This method uses the signature verification engine
		''' supplied by the given provider. Note that the specified Provider object
		''' does not have to be registered in the provider list.
		''' 
		''' This method was added to version 1.8 of the Java Platform Standard
		''' Edition. In order to maintain backwards compatibility with existing
		''' service providers, this method is not {@code abstract}
		''' and it provides a default implementation.
		''' </summary>
		''' <param name="key"> the PublicKey used to carry out the verification. </param>
		''' <param name="sigProvider"> the signature provider.
		''' </param>
		''' <exception cref="NoSuchAlgorithmException"> on unsupported signature
		''' algorithms. </exception>
		''' <exception cref="InvalidKeyException"> on incorrect key. </exception>
		''' <exception cref="SignatureException"> on signature errors. </exception>
		''' <exception cref="CRLException"> on encoding errors.
		''' @since 1.8 </exception>
		Public Overridable Sub verify(ByVal key As java.security.PublicKey, ByVal sigProvider As java.security.Provider)
			sun.security.x509.X509CRLImpl.verify(Me, key, sigProvider)
		End Sub

		''' <summary>
		''' Gets the {@code version} (version number) value from the CRL.
		''' The ASN.1 definition for this is:
		''' <pre>
		''' version    Version OPTIONAL,
		'''             -- if present, must be v2
		''' 
		''' Version  ::=  INTEGER  {  v1(0), v2(1), v3(2)  }
		'''             -- v3 does not apply to CRLs but appears for consistency
		'''             -- with definition of Version for certs
		''' </pre>
		''' </summary>
		''' <returns> the version number, i.e. 1 or 2. </returns>
		Public MustOverride ReadOnly Property version As Integer

		''' <summary>
		''' <strong>Denigrated</strong>, replaced by {@linkplain
		''' #getIssuerX500Principal()}. This method returns the {@code issuer}
		''' as an implementation specific Principal object, which should not be
		''' relied upon by portable code.
		''' 
		''' <p>
		''' Gets the {@code issuer} (issuer distinguished name) value from
		''' the CRL. The issuer name identifies the entity that signed (and
		''' issued) the CRL.
		''' 
		''' <p>The issuer name field contains an
		''' X.500 distinguished name (DN).
		''' The ASN.1 definition for this is:
		''' <pre>
		''' issuer    Name
		''' 
		''' Name ::= CHOICE { RDNSequence }
		''' RDNSequence ::= SEQUENCE OF RelativeDistinguishedName
		''' RelativeDistinguishedName ::=
		'''     SET OF AttributeValueAssertion
		''' 
		''' AttributeValueAssertion ::= SEQUENCE {
		'''                               AttributeType,
		'''                               AttributeValue }
		''' AttributeType ::= OBJECT IDENTIFIER
		''' AttributeValue ::= ANY
		''' </pre>
		''' The {@code Name} describes a hierarchical name composed of
		''' attributes,
		''' such as country name, and corresponding values, such as US.
		''' The type of the {@code AttributeValue} component is determined by
		''' the {@code AttributeType}; in general it will be a
		''' {@code directoryString}. A {@code directoryString} is usually
		''' one of {@code PrintableString},
		''' {@code TeletexString} or {@code UniversalString}.
		''' </summary>
		''' <returns> a Principal whose name is the issuer distinguished name. </returns>
		Public MustOverride ReadOnly Property issuerDN As java.security.Principal

		''' <summary>
		''' Returns the issuer (issuer distinguished name) value from the
		''' CRL as an {@code X500Principal}.
		''' <p>
		''' It is recommended that subclasses override this method.
		''' </summary>
		''' <returns> an {@code X500Principal} representing the issuer
		'''          distinguished name
		''' @since 1.4 </returns>
		Public Overridable Property issuerX500Principal As javax.security.auth.x500.X500Principal
			Get
				If issuerPrincipal Is Nothing Then issuerPrincipal = sun.security.x509.X509CRLImpl.getIssuerX500Principal(Me)
				Return issuerPrincipal
			End Get
		End Property

		''' <summary>
		''' Gets the {@code thisUpdate} date from the CRL.
		''' The ASN.1 definition for this is:
		''' <pre>
		''' thisUpdate   ChoiceOfTime
		''' ChoiceOfTime ::= CHOICE {
		'''     utcTime        UTCTime,
		'''     generalTime    GeneralizedTime }
		''' </pre>
		''' </summary>
		''' <returns> the {@code thisUpdate} date from the CRL. </returns>
		Public MustOverride ReadOnly Property thisUpdate As DateTime?

		''' <summary>
		''' Gets the {@code nextUpdate} date from the CRL.
		''' </summary>
		''' <returns> the {@code nextUpdate} date from the CRL, or null if
		''' not present. </returns>
		Public MustOverride ReadOnly Property nextUpdate As DateTime?

		''' <summary>
		''' Gets the CRL entry, if any, with the given certificate serialNumber.
		''' </summary>
		''' <param name="serialNumber"> the serial number of the certificate for which a CRL entry
		''' is to be looked up </param>
		''' <returns> the entry with the given serial number, or null if no such entry
		''' exists in this CRL. </returns>
		''' <seealso cref= X509CRLEntry </seealso>
		Public MustOverride Function getRevokedCertificate(ByVal serialNumber As System.Numerics.BigInteger) As X509CRLEntry

		''' <summary>
		''' Get the CRL entry, if any, for the given certificate.
		''' 
		''' <p>This method can be used to lookup CRL entries in indirect CRLs,
		''' that means CRLs that contain entries from issuers other than the CRL
		''' issuer. The default implementation will only return entries for
		''' certificates issued by the CRL issuer. Subclasses that wish to
		''' support indirect CRLs should override this method.
		''' </summary>
		''' <param name="certificate"> the certificate for which a CRL entry is to be looked
		'''   up </param>
		''' <returns> the entry for the given certificate, or null if no such entry
		'''   exists in this CRL. </returns>
		''' <exception cref="NullPointerException"> if certificate is null
		''' 
		''' @since 1.5 </exception>
		Public Overridable Function getRevokedCertificate(ByVal certificate As X509Certificate) As X509CRLEntry
			Dim certIssuer As javax.security.auth.x500.X500Principal = certificate.issuerX500Principal
			Dim crlIssuer As javax.security.auth.x500.X500Principal = issuerX500Principal
			If certIssuer.Equals(crlIssuer) = False Then Return Nothing
			Return getRevokedCertificate(certificate.serialNumber)
		End Function

		''' <summary>
		''' Gets all the entries from this CRL.
		''' This returns a Set of X509CRLEntry objects.
		''' </summary>
		''' <returns> all the entries or null if there are none present. </returns>
		''' <seealso cref= X509CRLEntry </seealso>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Public MustOverride ReadOnly Property revokedCertificates As java.util.Set(Of ? As X509CRLEntry)

		''' <summary>
		''' Gets the DER-encoded CRL information, the
		''' {@code tbsCertList} from this CRL.
		''' This can be used to verify the signature independently.
		''' </summary>
		''' <returns> the DER-encoded CRL information. </returns>
		''' <exception cref="CRLException"> if an encoding error occurs. </exception>
		Public MustOverride ReadOnly Property tBSCertList As SByte()

		''' <summary>
		''' Gets the {@code signature} value (the raw signature bits) from
		''' the CRL.
		''' The ASN.1 definition for this is:
		''' <pre>
		''' signature     BIT STRING
		''' </pre>
		''' </summary>
		''' <returns> the signature. </returns>
		Public MustOverride ReadOnly Property signature As SByte()

		''' <summary>
		''' Gets the signature algorithm name for the CRL
		''' signature algorithm. An example is the string "SHA256withRSA".
		''' The ASN.1 definition for this is:
		''' <pre>
		''' signatureAlgorithm   AlgorithmIdentifier
		''' 
		''' AlgorithmIdentifier  ::=  SEQUENCE  {
		'''     algorithm               OBJECT IDENTIFIER,
		'''     parameters              ANY DEFINED BY algorithm OPTIONAL  }
		'''                             -- contains a value of the type
		'''                             -- registered for use with the
		'''                             -- algorithm object identifier value
		''' </pre>
		''' 
		''' <p>The algorithm name is determined from the {@code algorithm}
		''' OID string.
		''' </summary>
		''' <returns> the signature algorithm name. </returns>
		Public MustOverride ReadOnly Property sigAlgName As String

		''' <summary>
		''' Gets the signature algorithm OID string from the CRL.
		''' An OID is represented by a set of nonnegative whole numbers separated
		''' by periods.
		''' For example, the string "1.2.840.10040.4.3" identifies the SHA-1
		''' with DSA signature algorithm defined in
		''' <a href="http://www.ietf.org/rfc/rfc3279.txt">RFC 3279: Algorithms and
		''' Identifiers for the Internet X.509 Public Key Infrastructure Certificate
		''' and CRL Profile</a>.
		''' 
		''' <p>See <seealso cref="#getSigAlgName() getSigAlgName"/> for
		''' relevant ASN.1 definitions.
		''' </summary>
		''' <returns> the signature algorithm OID string. </returns>
		Public MustOverride ReadOnly Property sigAlgOID As String

		''' <summary>
		''' Gets the DER-encoded signature algorithm parameters from this
		''' CRL's signature algorithm. In most cases, the signature
		''' algorithm parameters are null; the parameters are usually
		''' supplied with the public key.
		''' If access to individual parameter values is needed then use
		''' <seealso cref="java.security.AlgorithmParameters AlgorithmParameters"/>
		''' and instantiate with the name returned by
		''' <seealso cref="#getSigAlgName() getSigAlgName"/>.
		''' 
		''' <p>See <seealso cref="#getSigAlgName() getSigAlgName"/> for
		''' relevant ASN.1 definitions.
		''' </summary>
		''' <returns> the DER-encoded signature algorithm parameters, or
		'''         null if no parameters are present. </returns>
		Public MustOverride ReadOnly Property sigAlgParams As SByte()
	End Class

End Namespace