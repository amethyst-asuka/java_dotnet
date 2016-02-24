Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic

'
' * Copyright (c) 2000, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' A {@code CertSelector} that selects {@code X509Certificates} that
	''' match all specified criteria. This class is particularly useful when
	''' selecting certificates from a {@code CertStore} to build a
	''' PKIX-compliant certification path.
	''' <p>
	''' When first constructed, an {@code X509CertSelector} has no criteria
	''' enabled and each of the {@code get} methods return a default value
	''' ({@code null}, or {@code -1} for the {@link #getBasicConstraints
	''' getBasicConstraints} method). Therefore, the <seealso cref="#match match"/>
	''' method would return {@code true} for any {@code X509Certificate}.
	''' Typically, several criteria are enabled (by calling
	''' <seealso cref="#setIssuer setIssuer"/> or
	''' <seealso cref="#setKeyUsage setKeyUsage"/>, for instance) and then the
	''' {@code X509CertSelector} is passed to
	''' <seealso cref="CertStore#getCertificates CertStore.getCertificates"/> or some similar
	''' method.
	''' <p>
	''' Several criteria can be enabled (by calling <seealso cref="#setIssuer setIssuer"/>
	''' and <seealso cref="#setSerialNumber setSerialNumber"/>,
	''' for example) such that the {@code match} method
	''' usually uniquely matches a single {@code X509Certificate}. We say
	''' usually, since it is possible for two issuing CAs to have the same
	''' distinguished name and each issue a certificate with the same serial
	''' number. Other unique combinations include the issuer, subject,
	''' subjectKeyIdentifier and/or the subjectPublicKey criteria.
	''' <p>
	''' Please refer to <a href="http://www.ietf.org/rfc/rfc3280.txt">RFC 3280:
	''' Internet X.509 Public Key Infrastructure Certificate and CRL Profile</a> for
	''' definitions of the X.509 certificate extensions mentioned below.
	''' <p>
	''' <b>Concurrent Access</b>
	''' <p>
	''' Unless otherwise specified, the methods defined in this class are not
	''' thread-safe. Multiple threads that need to access a single
	''' object concurrently should synchronize amongst themselves and
	''' provide the necessary locking. Multiple threads each manipulating
	''' separate objects need not synchronize.
	''' </summary>
	''' <seealso cref= CertSelector </seealso>
	''' <seealso cref= X509Certificate
	''' 
	''' @since       1.4
	''' @author      Steve Hanna </seealso>
	Public Class X509CertSelector
		Implements CertSelector

		Private Shared ReadOnly debug As sun.security.util.Debug = sun.security.util.Debug.getInstance("certpath")

		Private Shared ReadOnly ANY_EXTENDED_KEY_USAGE As sun.security.util.ObjectIdentifier = sun.security.util.ObjectIdentifier.newInternal(New Integer() {2, 5, 29, 37, 0})

		Shared Sub New()
			CertPathHelperImpl.initialize()
			EXTENSION_OIDS(PRIVATE_KEY_USAGE_ID) = "2.5.29.16"
			EXTENSION_OIDS(SUBJECT_ALT_NAME_ID) = "2.5.29.17"
			EXTENSION_OIDS(NAME_CONSTRAINTS_ID) = "2.5.29.30"
			EXTENSION_OIDS(CERT_POLICIES_ID) = "2.5.29.32"
			EXTENSION_OIDS(EXTENDED_KEY_USAGE_ID) = "2.5.29.37"
		End Sub

		Private serialNumber As System.Numerics.BigInteger
		Private issuer As javax.security.auth.x500.X500Principal
		Private subject As javax.security.auth.x500.X500Principal
		Private subjectKeyID As SByte()
		Private authorityKeyID As SByte()
		Private certificateValid As Date
		Private privateKeyValid As Date
		Private subjectPublicKeyAlgID As sun.security.util.ObjectIdentifier
		Private subjectPublicKey As java.security.PublicKey
		Private subjectPublicKeyBytes As SByte()
		Private keyUsage As Boolean()
		Private keyPurposeSet As [Set](Of String)
		Private keyPurposeOIDSet As [Set](Of sun.security.util.ObjectIdentifier)
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Private subjectAlternativeNames As [Set](Of List(Of ?))
		Private subjectAlternativeGeneralNames As [Set](Of GeneralNameInterface)
		Private policy_Renamed As CertificatePolicySet
		Private policySet As [Set](Of String)
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Private pathToNames As [Set](Of List(Of ?))
		Private pathToGeneralNames As [Set](Of GeneralNameInterface)
		Private nc As NameConstraintsExtension
		Private ncBytes As SByte()
		Private basicConstraints As Integer = -1
		Private x509Cert As X509Certificate
		Private matchAllSubjectAltNames As Boolean = True

		Private Shared ReadOnly [FALSE] As Boolean? = Boolean.FALSE

		Private Const PRIVATE_KEY_USAGE_ID As Integer = 0
		Private Const SUBJECT_ALT_NAME_ID As Integer = 1
		Private Const NAME_CONSTRAINTS_ID As Integer = 2
		Private Const CERT_POLICIES_ID As Integer = 3
		Private Const EXTENDED_KEY_USAGE_ID As Integer = 4
		Private Const NUM_OF_EXTENSIONS As Integer = 5
		Private Shared ReadOnly EXTENSION_OIDS As String() = New String(NUM_OF_EXTENSIONS - 1){}


		' Constants representing the GeneralName types 
		Friend Const NAME_ANY As Integer = 0
		Friend Const NAME_RFC822 As Integer = 1
		Friend Const NAME_DNS As Integer = 2
		Friend Const NAME_X400 As Integer = 3
		Friend Const NAME_DIRECTORY As Integer = 4
		Friend Const NAME_EDI As Integer = 5
		Friend Const NAME_URI As Integer = 6
		Friend Const NAME_IP As Integer = 7
		Friend Const NAME_OID As Integer = 8

		''' <summary>
		''' Creates an {@code X509CertSelector}. Initially, no criteria are set
		''' so any {@code X509Certificate} will match.
		''' </summary>
		Public Sub New()
			' empty
		End Sub

		''' <summary>
		''' Sets the certificateEquals criterion. The specified
		''' {@code X509Certificate} must be equal to the
		''' {@code X509Certificate} passed to the {@code match} method.
		''' If {@code null}, then this check is not applied.
		''' 
		''' <p>This method is particularly useful when it is necessary to
		''' match a single certificate. Although other criteria can be specified
		''' in conjunction with the certificateEquals criterion, it is usually not
		''' practical or necessary.
		''' </summary>
		''' <param name="cert"> the {@code X509Certificate} to match (or
		''' {@code null}) </param>
		''' <seealso cref= #getCertificate </seealso>
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
        Public Overridable Sub setCertificate(ByVal cert As X509Certificate) 'JavaToDotNetTempPropertySetcertificate
		Public Overridable Property certificate As X509Certificate
			Set(ByVal cert As X509Certificate)
				x509Cert = cert
			End Set
			Get
		End Property

		''' <summary>
		''' Sets the serialNumber criterion. The specified serial number
		''' must match the certificate serial number in the
		''' {@code X509Certificate}. If {@code null}, any certificate
		''' serial number will do.
		''' </summary>
		''' <param name="serial"> the certificate serial number to match
		'''        (or {@code null}) </param>
		''' <seealso cref= #getSerialNumber </seealso>
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
        Public Overridable Sub setSerialNumber(ByVal serial As System.Numerics.BigInteger) 'JavaToDotNetTempPropertySetserialNumber
		Public Overridable Property serialNumber As System.Numerics.BigInteger
			Set(ByVal serial As System.Numerics.BigInteger)
				serialNumber = serial
			End Set
			Get
		End Property

		''' <summary>
		''' Sets the issuer criterion. The specified distinguished name
		''' must match the issuer distinguished name in the
		''' {@code X509Certificate}. If {@code null}, any issuer
		''' distinguished name will do.
		''' </summary>
		''' <param name="issuer"> a distinguished name as X500Principal
		'''                 (or {@code null})
		''' @since 1.5 </param>
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
        Public Overridable Sub setIssuer(ByVal issuer As javax.security.auth.x500.X500Principal) 'JavaToDotNetTempPropertySetissuer
		Public Overridable Property issuer As javax.security.auth.x500.X500Principal
			Set(ByVal issuer As javax.security.auth.x500.X500Principal)
				Me.issuer = issuer
			End Set
			Get
		End Property

		''' <summary>
		''' <strong>Denigrated</strong>, use <seealso cref="#setIssuer(X500Principal)"/>
		''' or <seealso cref="#setIssuer(byte[])"/> instead. This method should not be
		''' relied on as it can fail to match some certificates because of a loss of
		''' encoding information in the
		''' <a href="http://www.ietf.org/rfc/rfc2253.txt">RFC 2253</a> String form
		''' of some distinguished names.
		''' <p>
		''' Sets the issuer criterion. The specified distinguished name
		''' must match the issuer distinguished name in the
		''' {@code X509Certificate}. If {@code null}, any issuer
		''' distinguished name will do.
		''' <p>
		''' If {@code issuerDN} is not {@code null}, it should contain a
		''' distinguished name, in RFC 2253 format.
		''' </summary>
		''' <param name="issuerDN"> a distinguished name in RFC 2253 format
		'''                 (or {@code null}) </param>
		''' <exception cref="IOException"> if a parsing error occurs (incorrect form for DN) </exception>
		Public Overridable Property issuer As String
			Set(ByVal issuerDN As String)
				If issuerDN Is Nothing Then
					issuer = Nothing
				Else
					issuer = (New X500Name(issuerDN)).asX500Principal()
				End If
			End Set
		End Property

		''' <summary>
		''' Sets the issuer criterion. The specified distinguished name
		''' must match the issuer distinguished name in the
		''' {@code X509Certificate}. If {@code null} is specified,
		''' the issuer criterion is disabled and any issuer distinguished name will
		''' do.
		''' <p>
		''' If {@code issuerDN} is not {@code null}, it should contain a
		''' single DER encoded distinguished name, as defined in X.501. The ASN.1
		''' notation for this structure is as follows.
		''' <pre>{@code
		''' Name ::= CHOICE {
		'''   RDNSequence }
		''' 
		''' RDNSequence ::= SEQUENCE OF RelativeDistinguishedName
		''' 
		''' RelativeDistinguishedName ::=
		'''   SET SIZE (1 .. MAX) OF AttributeTypeAndValue
		''' 
		''' AttributeTypeAndValue ::= SEQUENCE {
		'''   type     AttributeType,
		'''   value    AttributeValue }
		''' 
		''' AttributeType ::= OBJECT IDENTIFIER
		''' 
		''' AttributeValue ::= ANY DEFINED BY AttributeType
		''' ....
		''' DirectoryString ::= CHOICE {
		'''       teletexString           TeletexString (SIZE (1..MAX)),
		'''       printableString         PrintableString (SIZE (1..MAX)),
		'''       universalString         UniversalString (SIZE (1..MAX)),
		'''       utf8String              UTF8String (SIZE (1.. MAX)),
		'''       bmpString               BMPString (SIZE (1..MAX)) }
		''' }</pre>
		''' <p>
		''' Note that the byte array specified here is cloned to protect against
		''' subsequent modifications.
		''' </summary>
		''' <param name="issuerDN"> a byte array containing the distinguished name
		'''                 in ASN.1 DER encoded form (or {@code null}) </param>
		''' <exception cref="IOException"> if an encoding error occurs (incorrect form for DN) </exception>
		Public Overridable Property issuer As SByte()
			Set(ByVal issuerDN As SByte())
				Try
					issuer = (If(issuerDN Is Nothing, Nothing, New javax.security.auth.x500.X500Principal(issuerDN)))
				Catch e As IllegalArgumentException
					Throw New java.io.IOException("Invalid name", e)
				End Try
			End Set
		End Property

		''' <summary>
		''' Sets the subject criterion. The specified distinguished name
		''' must match the subject distinguished name in the
		''' {@code X509Certificate}. If {@code null}, any subject
		''' distinguished name will do.
		''' </summary>
		''' <param name="subject"> a distinguished name as X500Principal
		'''                  (or {@code null})
		''' @since 1.5 </param>
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
        Public Overridable Sub setSubject(ByVal subject As javax.security.auth.x500.X500Principal) 'JavaToDotNetTempPropertySetsubject
		Public Overridable Property subject As javax.security.auth.x500.X500Principal
			Set(ByVal subject As javax.security.auth.x500.X500Principal)
				Me.subject = subject
			End Set
			Get
		End Property

		''' <summary>
		''' <strong>Denigrated</strong>, use <seealso cref="#setSubject(X500Principal)"/>
		''' or <seealso cref="#setSubject(byte[])"/> instead. This method should not be
		''' relied on as it can fail to match some certificates because of a loss of
		''' encoding information in the RFC 2253 String form of some distinguished
		''' names.
		''' <p>
		''' Sets the subject criterion. The specified distinguished name
		''' must match the subject distinguished name in the
		''' {@code X509Certificate}. If {@code null}, any subject
		''' distinguished name will do.
		''' <p>
		''' If {@code subjectDN} is not {@code null}, it should contain a
		''' distinguished name, in RFC 2253 format.
		''' </summary>
		''' <param name="subjectDN"> a distinguished name in RFC 2253 format
		'''                  (or {@code null}) </param>
		''' <exception cref="IOException"> if a parsing error occurs (incorrect form for DN) </exception>
		Public Overridable Property subject As String
			Set(ByVal subjectDN As String)
				If subjectDN Is Nothing Then
					subject = Nothing
				Else
					subject = (New X500Name(subjectDN)).asX500Principal()
				End If
			End Set
		End Property

		''' <summary>
		''' Sets the subject criterion. The specified distinguished name
		''' must match the subject distinguished name in the
		''' {@code X509Certificate}. If {@code null}, any subject
		''' distinguished name will do.
		''' <p>
		''' If {@code subjectDN} is not {@code null}, it should contain a
		''' single DER encoded distinguished name, as defined in X.501. For the ASN.1
		''' notation for this structure, see
		''' <seealso cref="#setIssuer(byte [] issuerDN) setIssuer(byte [] issuerDN)"/>.
		''' </summary>
		''' <param name="subjectDN"> a byte array containing the distinguished name in
		'''                  ASN.1 DER format (or {@code null}) </param>
		''' <exception cref="IOException"> if an encoding error occurs (incorrect form for DN) </exception>
		Public Overridable Property subject As SByte()
			Set(ByVal subjectDN As SByte())
				Try
					subject = (If(subjectDN Is Nothing, Nothing, New javax.security.auth.x500.X500Principal(subjectDN)))
				Catch e As IllegalArgumentException
					Throw New java.io.IOException("Invalid name", e)
				End Try
			End Set
		End Property

		''' <summary>
		''' Sets the subjectKeyIdentifier criterion. The
		''' {@code X509Certificate} must contain a SubjectKeyIdentifier
		''' extension for which the contents of the extension
		''' matches the specified criterion value.
		''' If the criterion value is {@code null}, no
		''' subjectKeyIdentifier check will be done.
		''' <p>
		''' If {@code subjectKeyID} is not {@code null}, it
		''' should contain a single DER encoded value corresponding to the contents
		''' of the extension value (not including the object identifier,
		''' criticality setting, and encapsulating OCTET STRING)
		''' for a SubjectKeyIdentifier extension.
		''' The ASN.1 notation for this structure follows.
		''' 
		''' <pre>{@code
		''' SubjectKeyIdentifier ::= KeyIdentifier
		''' 
		''' KeyIdentifier ::= OCTET STRING
		''' }</pre>
		''' <p>
		''' Since the format of subject key identifiers is not mandated by
		''' any standard, subject key identifiers are not parsed by the
		''' {@code X509CertSelector}. Instead, the values are compared using
		''' a byte-by-byte comparison.
		''' <p>
		''' Note that the byte array supplied here is cloned to protect against
		''' subsequent modifications.
		''' </summary>
		''' <param name="subjectKeyID"> the subject key identifier (or {@code null}) </param>
		''' <seealso cref= #getSubjectKeyIdentifier </seealso>
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
        Public Overridable Sub setSubjectKeyIdentifier(ByVal subjectKeyID As SByte[]) 'JavaToDotNetTempPropertySetsubjectKeyIdentifier
		Public Overridable Property subjectKeyIdentifier As SByte()
			Set(ByVal subjectKeyID As SByte())
				If subjectKeyID Is Nothing Then
					Me.subjectKeyID = Nothing
				Else
					Me.subjectKeyID = subjectKeyID.clone()
				End If
			End Set
			Get
		End Property

		''' <summary>
		''' Sets the authorityKeyIdentifier criterion. The
		''' {@code X509Certificate} must contain an
		''' AuthorityKeyIdentifier extension for which the contents of the
		''' extension value matches the specified criterion value.
		''' If the criterion value is {@code null}, no
		''' authorityKeyIdentifier check will be done.
		''' <p>
		''' If {@code authorityKeyID} is not {@code null}, it
		''' should contain a single DER encoded value corresponding to the contents
		''' of the extension value (not including the object identifier,
		''' criticality setting, and encapsulating OCTET STRING)
		''' for an AuthorityKeyIdentifier extension.
		''' The ASN.1 notation for this structure follows.
		''' 
		''' <pre>{@code
		''' AuthorityKeyIdentifier ::= SEQUENCE {
		'''    keyIdentifier             [0] KeyIdentifier           OPTIONAL,
		'''    authorityCertIssuer       [1] GeneralNames            OPTIONAL,
		'''    authorityCertSerialNumber [2] CertificateSerialNumber OPTIONAL  }
		''' 
		''' KeyIdentifier ::= OCTET STRING
		''' }</pre>
		''' <p>
		''' Authority key identifiers are not parsed by the
		''' {@code X509CertSelector}.  Instead, the values are
		''' compared using a byte-by-byte comparison.
		''' <p>
		''' When the {@code keyIdentifier} field of
		''' {@code AuthorityKeyIdentifier} is populated, the value is
		''' usually taken from the {@code SubjectKeyIdentifier} extension
		''' in the issuer's certificate.  Note, however, that the result of
		''' {@code X509Certificate.getExtensionValue(<SubjectKeyIdentifier Object
		''' Identifier>)} on the issuer's certificate may NOT be used
		''' directly as the input to {@code setAuthorityKeyIdentifier}.
		''' This is because the SubjectKeyIdentifier contains
		''' only a KeyIdentifier OCTET STRING, and not a SEQUENCE of
		''' KeyIdentifier, GeneralNames, and CertificateSerialNumber.
		''' In order to use the extension value of the issuer certificate's
		''' {@code SubjectKeyIdentifier}
		''' extension, it will be necessary to extract the value of the embedded
		''' {@code KeyIdentifier} OCTET STRING, then DER encode this OCTET
		''' STRING inside a SEQUENCE.
		''' For more details on SubjectKeyIdentifier, see
		''' <seealso cref="#setSubjectKeyIdentifier(byte[] subjectKeyID)"/>.
		''' <p>
		''' Note also that the byte array supplied here is cloned to protect against
		''' subsequent modifications.
		''' </summary>
		''' <param name="authorityKeyID"> the authority key identifier
		'''        (or {@code null}) </param>
		''' <seealso cref= #getAuthorityKeyIdentifier </seealso>
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
        Public Overridable Sub setAuthorityKeyIdentifier(ByVal authorityKeyID As SByte[]) 'JavaToDotNetTempPropertySetauthorityKeyIdentifier
		Public Overridable Property authorityKeyIdentifier As SByte()
			Set(ByVal authorityKeyID As SByte())
				If authorityKeyID Is Nothing Then
					Me.authorityKeyID = Nothing
				Else
					Me.authorityKeyID = authorityKeyID.clone()
				End If
			End Set
			Get
		End Property

		''' <summary>
		''' Sets the certificateValid criterion. The specified date must fall
		''' within the certificate validity period for the
		''' {@code X509Certificate}. If {@code null}, no certificateValid
		''' check will be done.
		''' <p>
		''' Note that the {@code Date} supplied here is cloned to protect
		''' against subsequent modifications.
		''' </summary>
		''' <param name="certValid"> the {@code Date} to check (or {@code null}) </param>
		''' <seealso cref= #getCertificateValid </seealso>
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
        Public Overridable Sub setCertificateValid(ByVal certValid As Date) 'JavaToDotNetTempPropertySetcertificateValid
		Public Overridable Property certificateValid As Date
			Set(ByVal certValid As Date)
				If certValid Is Nothing Then
					certificateValid = Nothing
				Else
					certificateValid = CDate(certValid.clone())
				End If
			End Set
			Get
		End Property

		''' <summary>
		''' Sets the privateKeyValid criterion. The specified date must fall
		''' within the private key validity period for the
		''' {@code X509Certificate}. If {@code null}, no privateKeyValid
		''' check will be done.
		''' <p>
		''' Note that the {@code Date} supplied here is cloned to protect
		''' against subsequent modifications.
		''' </summary>
		''' <param name="privateKeyValid"> the {@code Date} to check (or
		'''                        {@code null}) </param>
		''' <seealso cref= #getPrivateKeyValid </seealso>
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
        Public Overridable Sub setPrivateKeyValid(ByVal privateKeyValid As Date) 'JavaToDotNetTempPropertySetprivateKeyValid
		Public Overridable Property privateKeyValid As Date
			Set(ByVal privateKeyValid As Date)
				If privateKeyValid Is Nothing Then
					Me.privateKeyValid = Nothing
				Else
					Me.privateKeyValid = CDate(privateKeyValid.clone())
				End If
			End Set
			Get
		End Property

		''' <summary>
		''' Sets the subjectPublicKeyAlgID criterion. The
		''' {@code X509Certificate} must contain a subject public key
		''' with the specified algorithm. If {@code null}, no
		''' subjectPublicKeyAlgID check will be done.
		''' </summary>
		''' <param name="oid"> The object identifier (OID) of the algorithm to check
		'''            for (or {@code null}). An OID is represented by a
		'''            set of nonnegative integers separated by periods. </param>
		''' <exception cref="IOException"> if the OID is invalid, such as
		''' the first component being not 0, 1 or 2 or the second component
		''' being greater than 39.
		''' </exception>
		''' <seealso cref= #getSubjectPublicKeyAlgID </seealso>
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
        Public Overridable Sub setSubjectPublicKeyAlgID(ByVal oid As String) 'JavaToDotNetTempPropertySetsubjectPublicKeyAlgID
		Public Overridable Property subjectPublicKeyAlgID As String
			Set(ByVal oid As String)
				If oid Is Nothing Then
					subjectPublicKeyAlgID = Nothing
				Else
					subjectPublicKeyAlgID = New sun.security.util.ObjectIdentifier(oid)
				End If
			End Set
			Get
		End Property

		''' <summary>
		''' Sets the subjectPublicKey criterion. The
		''' {@code X509Certificate} must contain the specified subject public
		''' key. If {@code null}, no subjectPublicKey check will be done.
		''' </summary>
		''' <param name="key"> the subject public key to check for (or {@code null}) </param>
		''' <seealso cref= #getSubjectPublicKey </seealso>
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
        Public Overridable Sub setSubjectPublicKey(ByVal key As java.security.PublicKey) 'JavaToDotNetTempPropertySetsubjectPublicKey
		Public Overridable Property subjectPublicKey As java.security.PublicKey
			Set(ByVal key As java.security.PublicKey)
				If key Is Nothing Then
					subjectPublicKey = Nothing
					subjectPublicKeyBytes = Nothing
				Else
					subjectPublicKey = key
					subjectPublicKeyBytes = key.encoded
				End If
			End Set
			Get
		End Property

		''' <summary>
		''' Sets the subjectPublicKey criterion. The {@code X509Certificate}
		''' must contain the specified subject public key. If {@code null},
		''' no subjectPublicKey check will be done.
		''' <p>
		''' Because this method allows the public key to be specified as a byte
		''' array, it may be used for unknown key types.
		''' <p>
		''' If {@code key} is not {@code null}, it should contain a
		''' single DER encoded SubjectPublicKeyInfo structure, as defined in X.509.
		''' The ASN.1 notation for this structure is as follows.
		''' <pre>{@code
		''' SubjectPublicKeyInfo  ::=  SEQUENCE  {
		'''   algorithm            AlgorithmIdentifier,
		'''   subjectPublicKey     BIT STRING  }
		''' 
		''' AlgorithmIdentifier  ::=  SEQUENCE  {
		'''   algorithm               OBJECT IDENTIFIER,
		'''   parameters              ANY DEFINED BY algorithm OPTIONAL  }
		'''                              -- contains a value of the type
		'''                              -- registered for use with the
		'''                              -- algorithm object identifier value
		''' }</pre>
		''' <p>
		''' Note that the byte array supplied here is cloned to protect against
		''' subsequent modifications.
		''' </summary>
		''' <param name="key"> a byte array containing the subject public key in ASN.1 DER
		'''            form (or {@code null}) </param>
		''' <exception cref="IOException"> if an encoding error occurs (incorrect form for
		''' subject public key) </exception>
		''' <seealso cref= #getSubjectPublicKey </seealso>
		Public Overridable Property subjectPublicKey As SByte()
			Set(ByVal key As SByte())
				If key Is Nothing Then
					subjectPublicKey = Nothing
					subjectPublicKeyBytes = Nothing
				Else
					subjectPublicKeyBytes = key.clone()
					subjectPublicKey = X509Key.parse(New sun.security.util.DerValue(subjectPublicKeyBytes))
				End If
			End Set
		End Property

		''' <summary>
		''' Sets the keyUsage criterion. The {@code X509Certificate}
		''' must allow the specified keyUsage values. If {@code null}, no
		''' keyUsage check will be done. Note that an {@code X509Certificate}
		''' that has no keyUsage extension implicitly allows all keyUsage values.
		''' <p>
		''' Note that the boolean array supplied here is cloned to protect against
		''' subsequent modifications.
		''' </summary>
		''' <param name="keyUsage"> a boolean array in the same format as the boolean
		'''                 array returned by
		''' <seealso cref="X509Certificate#getKeyUsage() X509Certificate.getKeyUsage()"/>.
		'''                 Or {@code null}. </param>
		''' <seealso cref= #getKeyUsage </seealso>
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
        Public Overridable Sub setKeyUsage(ByVal keyUsage As Boolean[]) 'JavaToDotNetTempPropertySetkeyUsage
		Public Overridable Property keyUsage As Boolean()
			Set(ByVal keyUsage As Boolean())
				If keyUsage Is Nothing Then
					Me.keyUsage = Nothing
				Else
					Me.keyUsage = keyUsage.clone()
				End If
			End Set
			Get
		End Property

		''' <summary>
		''' Sets the extendedKeyUsage criterion. The {@code X509Certificate}
		''' must allow the specified key purposes in its extended key usage
		''' extension. If {@code keyPurposeSet} is empty or {@code null},
		''' no extendedKeyUsage check will be done. Note that an
		''' {@code X509Certificate} that has no extendedKeyUsage extension
		''' implicitly allows all key purposes.
		''' <p>
		''' Note that the {@code Set} is cloned to protect against
		''' subsequent modifications.
		''' </summary>
		''' <param name="keyPurposeSet"> a {@code Set} of key purpose OIDs in string
		''' format (or {@code null}). Each OID is represented by a set of
		''' nonnegative integers separated by periods. </param>
		''' <exception cref="IOException"> if the OID is invalid, such as
		''' the first component being not 0, 1 or 2 or the second component
		''' being greater than 39. </exception>
		''' <seealso cref= #getExtendedKeyUsage </seealso>
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
        Public Overridable Sub setExtendedKeyUsage(ByVal keyPurposeSet As Set(Of String)) 'JavaToDotNetTempPropertySetextendedKeyUsage
		Public Overridable Property extendedKeyUsage As [Set](Of String)
			Set(ByVal keyPurposeSet As [Set](Of String))
				If (keyPurposeSet Is Nothing) OrElse keyPurposeSet.empty Then
					Me.keyPurposeSet = Nothing
					keyPurposeOIDSet = Nothing
				Else
					Me.keyPurposeSet = Collections.unmodifiableSet(New HashSet(Of String)(keyPurposeSet))
					keyPurposeOIDSet = New HashSet(Of sun.security.util.ObjectIdentifier)
					For Each s As String In Me.keyPurposeSet
						keyPurposeOIDSet.add(New sun.security.util.ObjectIdentifier(s))
					Next s
				End If
			End Set
			Get
		End Property

		''' <summary>
		''' Enables/disables matching all of the subjectAlternativeNames
		''' specified in the {@link #setSubjectAlternativeNames
		''' setSubjectAlternativeNames} or {@link #addSubjectAlternativeName
		''' addSubjectAlternativeName} methods. If enabled,
		''' the {@code X509Certificate} must contain all of the
		''' specified subject alternative names. If disabled, the
		''' {@code X509Certificate} must contain at least one of the
		''' specified subject alternative names.
		''' 
		''' <p>The matchAllNames flag is {@code true} by default.
		''' </summary>
		''' <param name="matchAllNames"> if {@code true}, the flag is enabled;
		''' if {@code false}, the flag is disabled. </param>
		''' <seealso cref= #getMatchAllSubjectAltNames </seealso>
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
        Public Overridable Sub setMatchAllSubjectAltNames(ByVal matchAllNames As Boolean) 'JavaToDotNetTempPropertySetmatchAllSubjectAltNames
		Public Overridable Property matchAllSubjectAltNames As Boolean
			Set(ByVal matchAllNames As Boolean)
				Me.matchAllSubjectAltNames = matchAllNames
			End Set
			Get
		End Property

		''' <summary>
		''' Sets the subjectAlternativeNames criterion. The
		''' {@code X509Certificate} must contain all or at least one of the
		''' specified subjectAlternativeNames, depending on the value of
		''' the matchAllNames flag (see {@link #setMatchAllSubjectAltNames
		''' setMatchAllSubjectAltNames}).
		''' <p>
		''' This method allows the caller to specify, with a single method call,
		''' the complete set of subject alternative names for the
		''' subjectAlternativeNames criterion. The specified value replaces
		''' the previous value for the subjectAlternativeNames criterion.
		''' <p>
		''' The {@code names} parameter (if not {@code null}) is a
		''' {@code Collection} with one
		''' entry for each name to be included in the subject alternative name
		''' criterion. Each entry is a {@code List} whose first entry is an
		''' {@code Integer} (the name type, 0-8) and whose second
		''' entry is a {@code String} or a byte array (the name, in
		''' string or ASN.1 DER encoded form, respectively).
		''' There can be multiple names of the same type. If {@code null}
		''' is supplied as the value for this argument, no
		''' subjectAlternativeNames check will be performed.
		''' <p>
		''' Each subject alternative name in the {@code Collection}
		''' may be specified either as a {@code String} or as an ASN.1 encoded
		''' byte array. For more details about the formats used, see
		''' {@link #addSubjectAlternativeName(int type, String name)
		''' addSubjectAlternativeName(int type, String name)} and
		''' {@link #addSubjectAlternativeName(int type, byte [] name)
		''' addSubjectAlternativeName(int type, byte [] name)}.
		''' <p>
		''' <strong>Note:</strong> for distinguished names, specify the byte
		''' array form instead of the String form. See the note in
		''' <seealso cref="#addSubjectAlternativeName(int, String)"/> for more information.
		''' <p>
		''' Note that the {@code names} parameter can contain duplicate
		''' names (same name and name type), but they may be removed from the
		''' {@code Collection} of names returned by the
		''' <seealso cref="#getSubjectAlternativeNames getSubjectAlternativeNames"/> method.
		''' <p>
		''' Note that a deep copy is performed on the {@code Collection} to
		''' protect against subsequent modifications.
		''' </summary>
		''' <param name="names"> a {@code Collection} of names (or {@code null}) </param>
		''' <exception cref="IOException"> if a parsing error occurs </exception>
		''' <seealso cref= #getSubjectAlternativeNames </seealso>
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
        Public Overridable Sub setSubjectAlternativeNames(Of T1)(ByVal names As Collection(Of T1)) 'JavaToDotNetTempPropertySetsubjectAlternativeNames
		Public Overridable Property subjectAlternativeNames(Of T1) As Collection(Of T1)
			Set(ByVal names As Collection(Of T1))
				If names Is Nothing Then
					subjectAlternativeNames = Nothing
					subjectAlternativeGeneralNames = Nothing
				Else
					If names.empty Then
						subjectAlternativeNames = Nothing
						subjectAlternativeGeneralNames = Nothing
						Return
					End If
	'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
					Dim tempNames As [Set](Of List(Of ?)) = cloneAndCheckNames(names)
					' Ensure that we either set both of these or neither
					subjectAlternativeGeneralNames = parseNames(tempNames)
					subjectAlternativeNames = tempNames
				End If
			End Set
			Get
		End Property

		''' <summary>
		''' Adds a name to the subjectAlternativeNames criterion. The
		''' {@code X509Certificate} must contain all or at least one
		''' of the specified subjectAlternativeNames, depending on the value of
		''' the matchAllNames flag (see {@link #setMatchAllSubjectAltNames
		''' setMatchAllSubjectAltNames}).
		''' <p>
		''' This method allows the caller to add a name to the set of subject
		''' alternative names.
		''' The specified name is added to any previous value for the
		''' subjectAlternativeNames criterion. If the specified name is a
		''' duplicate, it may be ignored.
		''' <p>
		''' The name is provided in string format.
		''' <a href="http://www.ietf.org/rfc/rfc822.txt">RFC 822</a>, DNS, and URI
		''' names use the well-established string formats for those types (subject to
		''' the restrictions included in RFC 3280). IPv4 address names are
		''' supplied using dotted quad notation. OID address names are represented
		''' as a series of nonnegative integers separated by periods. And
		''' directory names (distinguished names) are supplied in RFC 2253 format.
		''' No standard string format is defined for otherNames, X.400 names,
		''' EDI party names, IPv6 address names, or any other type of names. They
		''' should be specified using the
		''' {@link #addSubjectAlternativeName(int type, byte [] name)
		''' addSubjectAlternativeName(int type, byte [] name)}
		''' method.
		''' <p>
		''' <strong>Note:</strong> for distinguished names, use
		''' <seealso cref="#addSubjectAlternativeName(int, byte[])"/> instead.
		''' This method should not be relied on as it can fail to match some
		''' certificates because of a loss of encoding information in the RFC 2253
		''' String form of some distinguished names.
		''' </summary>
		''' <param name="type"> the name type (0-8, as specified in
		'''             RFC 3280, section 4.2.1.7) </param>
		''' <param name="name"> the name in string form (not {@code null}) </param>
		''' <exception cref="IOException"> if a parsing error occurs </exception>
		Public Overridable Sub addSubjectAlternativeName(ByVal type As Integer, ByVal name As String)
			addSubjectAlternativeNameInternal(type, name)
		End Sub

		''' <summary>
		''' Adds a name to the subjectAlternativeNames criterion. The
		''' {@code X509Certificate} must contain all or at least one
		''' of the specified subjectAlternativeNames, depending on the value of
		''' the matchAllNames flag (see {@link #setMatchAllSubjectAltNames
		''' setMatchAllSubjectAltNames}).
		''' <p>
		''' This method allows the caller to add a name to the set of subject
		''' alternative names.
		''' The specified name is added to any previous value for the
		''' subjectAlternativeNames criterion. If the specified name is a
		''' duplicate, it may be ignored.
		''' <p>
		''' The name is provided as a byte array. This byte array should contain
		''' the DER encoded name, as it would appear in the GeneralName structure
		''' defined in RFC 3280 and X.509. The encoded byte array should only contain
		''' the encoded value of the name, and should not include the tag associated
		''' with the name in the GeneralName structure. The ASN.1 definition of this
		''' structure appears below.
		''' <pre>{@code
		'''  GeneralName ::= CHOICE {
		'''       otherName                       [0]     OtherName,
		'''       rfc822Name                      [1]     IA5String,
		'''       dNSName                         [2]     IA5String,
		'''       x400Address                     [3]     ORAddress,
		'''       directoryName                   [4]     Name,
		'''       ediPartyName                    [5]     EDIPartyName,
		'''       uniformResourceIdentifier       [6]     IA5String,
		'''       iPAddress                       [7]     OCTET STRING,
		'''       registeredID                    [8]     OBJECT IDENTIFIER}
		''' }</pre>
		''' <p>
		''' Note that the byte array supplied here is cloned to protect against
		''' subsequent modifications.
		''' </summary>
		''' <param name="type"> the name type (0-8, as listed above) </param>
		''' <param name="name"> a byte array containing the name in ASN.1 DER encoded form </param>
		''' <exception cref="IOException"> if a parsing error occurs </exception>
		Public Overridable Sub addSubjectAlternativeName(ByVal type As Integer, ByVal name As SByte())
			' clone because byte arrays are modifiable
			addSubjectAlternativeNameInternal(type, name.clone())
		End Sub

		''' <summary>
		''' A private method that adds a name (String or byte array) to the
		''' subjectAlternativeNames criterion. The {@code X509Certificate}
		''' must contain the specified subjectAlternativeName.
		''' </summary>
		''' <param name="type"> the name type (0-8, as specified in
		'''             RFC 3280, section 4.2.1.7) </param>
		''' <param name="name"> the name in string or byte array form </param>
		''' <exception cref="IOException"> if a parsing error occurs </exception>
		Private Sub addSubjectAlternativeNameInternal(ByVal type As Integer, ByVal name As Object)
			' First, ensure that the name parses
			Dim tempName As GeneralNameInterface = makeGeneralNameInterface(type, name)
			If subjectAlternativeNames Is Nothing Then subjectAlternativeNames = New HashSet(Of List(Of ?))
			If subjectAlternativeGeneralNames Is Nothing Then subjectAlternativeGeneralNames = New HashSet(Of GeneralNameInterface)
			Dim list As List(Of Object) = New List(Of Object)(2)
			list.add(Convert.ToInt32(type))
			list.add(name)
			subjectAlternativeNames.add(list)
			subjectAlternativeGeneralNames.add(tempName)
		End Sub

		''' <summary>
		''' Parse an argument of the form passed to setSubjectAlternativeNames,
		''' returning a {@code Collection} of
		''' {@code GeneralNameInterface}s.
		''' Throw an IllegalArgumentException or a ClassCastException
		''' if the argument is malformed.
		''' </summary>
		''' <param name="names"> a Collection with one entry per name.
		'''              Each entry is a {@code List} whose first entry
		'''              is an Integer (the name type, 0-8) and whose second
		'''              entry is a String or a byte array (the name, in
		'''              string or ASN.1 DER encoded form, respectively).
		'''              There can be multiple names of the same type. Null is
		'''              not an acceptable value. </param>
		''' <returns> a Set of {@code GeneralNameInterface}s </returns>
		''' <exception cref="IOException"> if a parsing error occurs </exception>
		Private Shared Function parseNames(Of T1)(ByVal names As Collection(Of T1)) As [Set](Of GeneralNameInterface)
			Dim genNames As [Set](Of GeneralNameInterface) = New HashSet(Of GeneralNameInterface)
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			For Each nameList As List(Of ?) In names
				If nameList.size() <> 2 Then Throw New java.io.IOException("name list size not 2")
				Dim o As Object = nameList.get(0)
				If Not(TypeOf o Is Integer?) Then Throw New java.io.IOException("expected an Integer")
				Dim nameType As Integer = CInt(Fix(o))
				o = nameList.get(1)
				genNames.add(makeGeneralNameInterface(nameType, o))
			Next nameList

			Return genNames
		End Function

		''' <summary>
		''' Compare for equality two objects of the form passed to
		''' setSubjectAlternativeNames (or X509CRLSelector.setIssuerNames).
		''' Throw an {@code IllegalArgumentException} or a
		''' {@code ClassCastException} if one of the objects is malformed.
		''' </summary>
		''' <param name="object1"> a Collection containing the first object to compare </param>
		''' <param name="object2"> a Collection containing the second object to compare </param>
		''' <returns> true if the objects are equal, false otherwise </returns>
		Friend Shared Function equalNames(Of T1, T2)(ByVal object1 As Collection(Of T1), ByVal object2 As Collection(Of T2)) As Boolean
			If (object1 Is Nothing) OrElse (object2 Is Nothing) Then Return object1 Is object2
			Return object1.Equals(object2)
		End Function

		''' <summary>
		''' Make a {@code GeneralNameInterface} out of a name type (0-8) and an
		''' Object that may be a byte array holding the ASN.1 DER encoded
		''' name or a String form of the name.  Except for X.509
		''' Distinguished Names, the String form of the name must not be the
		''' result from calling toString on an existing GeneralNameInterface
		''' implementing class.  The output of toString is not compatible
		''' with the String constructors for names other than Distinguished
		''' Names.
		''' </summary>
		''' <param name="type"> name type (0-8) </param>
		''' <param name="name"> name as ASN.1 Der-encoded byte array or String </param>
		''' <returns> a GeneralNameInterface name </returns>
		''' <exception cref="IOException"> if a parsing error occurs </exception>
		Friend Shared Function makeGeneralNameInterface(ByVal type As Integer, ByVal name As Object) As GeneralNameInterface
			Dim result As GeneralNameInterface
			If debug IsNot Nothing Then debug.println("X509CertSelector.makeGeneralNameInterface(" & type & ")...")

			If TypeOf name Is String Then
				If debug IsNot Nothing Then debug.println("X509CertSelector.makeGeneralNameInterface() " & "name is String: " & name)
				Select Case type
				Case NAME_RFC822
					result = New RFC822Name(CStr(name))
				Case NAME_DNS
					result = New DNSName(CStr(name))
				Case NAME_DIRECTORY
					result = New X500Name(CStr(name))
				Case NAME_URI
					result = New URIName(CStr(name))
				Case NAME_IP
					result = New IPAddressName(CStr(name))
				Case NAME_OID
					result = New OIDName(CStr(name))
				Case Else
					Throw New java.io.IOException("unable to parse String names of type " & type)
				End Select
				If debug IsNot Nothing Then debug.println("X509CertSelector.makeGeneralNameInterface() " & "result: " & result.ToString())
			ElseIf TypeOf name Is SByte() Then
				Dim val As New sun.security.util.DerValue(CType(name, SByte()))
				If debug IsNot Nothing Then debug.println("X509CertSelector.makeGeneralNameInterface() is byte[]")

				Select Case type
				Case NAME_ANY
					result = New OtherName(val)
				Case NAME_RFC822
					result = New RFC822Name(val)
				Case NAME_DNS
					result = New DNSName(val)
				Case NAME_X400
					result = New X400Address(val)
				Case NAME_DIRECTORY
					result = New X500Name(val)
				Case NAME_EDI
					result = New EDIPartyName(val)
				Case NAME_URI
					result = New URIName(val)
				Case NAME_IP
					result = New IPAddressName(val)
				Case NAME_OID
					result = New OIDName(val)
				Case Else
					Throw New java.io.IOException("unable to parse byte array names of " & "type " & type)
				End Select
				If debug IsNot Nothing Then debug.println("X509CertSelector.makeGeneralNameInterface() result: " & result.ToString())
			Else
				If debug IsNot Nothing Then debug.println("X509CertSelector.makeGeneralName() input name " & "not String or byte array")
				Throw New java.io.IOException("name not String or byte array")
			End If
			Return result
		End Function


		''' <summary>
		''' Sets the name constraints criterion. The {@code X509Certificate}
		''' must have subject and subject alternative names that
		''' meet the specified name constraints.
		''' <p>
		''' The name constraints are specified as a byte array. This byte array
		''' should contain the DER encoded form of the name constraints, as they
		''' would appear in the NameConstraints structure defined in RFC 3280
		''' and X.509. The ASN.1 definition of this structure appears below.
		''' 
		''' <pre>{@code
		'''  NameConstraints ::= SEQUENCE {
		'''       permittedSubtrees       [0]     GeneralSubtrees OPTIONAL,
		'''       excludedSubtrees        [1]     GeneralSubtrees OPTIONAL }
		''' 
		'''  GeneralSubtrees ::= SEQUENCE SIZE (1..MAX) OF GeneralSubtree
		''' 
		'''  GeneralSubtree ::= SEQUENCE {
		'''       base                    GeneralName,
		'''       minimum         [0]     BaseDistance DEFAULT 0,
		'''       maximum         [1]     BaseDistance OPTIONAL }
		''' 
		'''  BaseDistance ::= INTEGER (0..MAX)
		''' 
		'''  GeneralName ::= CHOICE {
		'''       otherName                       [0]     OtherName,
		'''       rfc822Name                      [1]     IA5String,
		'''       dNSName                         [2]     IA5String,
		'''       x400Address                     [3]     ORAddress,
		'''       directoryName                   [4]     Name,
		'''       ediPartyName                    [5]     EDIPartyName,
		'''       uniformResourceIdentifier       [6]     IA5String,
		'''       iPAddress                       [7]     OCTET STRING,
		'''       registeredID                    [8]     OBJECT IDENTIFIER}
		''' }</pre>
		''' <p>
		''' Note that the byte array supplied here is cloned to protect against
		''' subsequent modifications.
		''' </summary>
		''' <param name="bytes"> a byte array containing the ASN.1 DER encoding of
		'''              a NameConstraints extension to be used for checking
		'''              name constraints. Only the value of the extension is
		'''              included, not the OID or criticality flag. Can be
		'''              {@code null},
		'''              in which case no name constraints check will be performed. </param>
		''' <exception cref="IOException"> if a parsing error occurs </exception>
		''' <seealso cref= #getNameConstraints </seealso>
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
        Public Overridable Sub setNameConstraints(ByVal bytes As SByte[]) 'JavaToDotNetTempPropertySetnameConstraints
		Public Overridable Property nameConstraints As SByte()
			Set(ByVal bytes As SByte())
				If bytes Is Nothing Then
					ncBytes = Nothing
					nc = Nothing
				Else
					ncBytes = bytes.clone()
					nc = New NameConstraintsExtension([FALSE], bytes)
				End If
			End Set
			Get
		End Property

		''' <summary>
		''' Sets the basic constraints constraint. If the value is greater than or
		''' equal to zero, {@code X509Certificates} must include a
		''' basicConstraints extension with
		''' a pathLen of at least this value. If the value is -2, only end-entity
		''' certificates are accepted. If the value is -1, no check is done.
		''' <p>
		''' This constraint is useful when building a certification path forward
		''' (from the target toward the trust anchor. If a partial path has been
		''' built, any candidate certificate must have a maxPathLen value greater
		''' than or equal to the number of certificates in the partial path.
		''' </summary>
		''' <param name="minMaxPathLen"> the value for the basic constraints constraint </param>
		''' <exception cref="IllegalArgumentException"> if the value is less than -2 </exception>
		''' <seealso cref= #getBasicConstraints </seealso>
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
        Public Overridable Sub setBasicConstraints(ByVal minMaxPathLen As Integer) 'JavaToDotNetTempPropertySetbasicConstraints
		Public Overridable Property basicConstraints As Integer
			Set(ByVal minMaxPathLen As Integer)
				If minMaxPathLen < -2 Then Throw New IllegalArgumentException("basic constraints less than -2")
				basicConstraints = minMaxPathLen
			End Set
			Get
		End Property

		''' <summary>
		''' Sets the policy constraint. The {@code X509Certificate} must
		''' include at least one of the specified policies in its certificate
		''' policies extension. If {@code certPolicySet} is empty, then the
		''' {@code X509Certificate} must include at least some specified policy
		''' in its certificate policies extension. If {@code certPolicySet} is
		''' {@code null}, no policy check will be performed.
		''' <p>
		''' Note that the {@code Set} is cloned to protect against
		''' subsequent modifications.
		''' </summary>
		''' <param name="certPolicySet"> a {@code Set} of certificate policy OIDs in
		'''                      string format (or {@code null}). Each OID is
		'''                      represented by a set of nonnegative integers
		'''                    separated by periods. </param>
		''' <exception cref="IOException"> if a parsing error occurs on the OID such as
		''' the first component is not 0, 1 or 2 or the second component is
		''' greater than 39. </exception>
		''' <seealso cref= #getPolicy </seealso>
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
        Public Overridable Sub setPolicy(ByVal certPolicySet As Set(Of String)) 'JavaToDotNetTempPropertySetpolicy
		Public Overridable Property policy As [Set](Of String)
			Set(ByVal certPolicySet As [Set](Of String))
				If certPolicySet Is Nothing Then
					policySet = Nothing
					policy_Renamed = Nothing
				Else
					' Snapshot set and parse it
					Dim tempSet As [Set](Of String) = Collections.unmodifiableSet(New HashSet(Of String)(certPolicySet))
					' Convert to Vector of ObjectIdentifiers 
					Dim i As [Iterator](Of String) = tempSet.GetEnumerator()
					Dim polIdVector As New Vector(Of CertificatePolicyId)
					Do While i.MoveNext()
						Dim o As Object = i.Current
						If Not(TypeOf o Is String) Then Throw New java.io.IOException("non String in certPolicySet")
						polIdVector.add(New CertificatePolicyId(New sun.security.util.ObjectIdentifier(CStr(o))))
					Loop
					' If everything went OK, make the changes
					policySet = tempSet
					policy_Renamed = New CertificatePolicySet(polIdVector)
				End If
			End Set
			Get
		End Property

		''' <summary>
		''' Sets the pathToNames criterion. The {@code X509Certificate} must
		''' not include name constraints that would prohibit building a
		''' path to the specified names.
		''' <p>
		''' This method allows the caller to specify, with a single method call,
		''' the complete set of names which the {@code X509Certificates}'s
		''' name constraints must permit. The specified value replaces
		''' the previous value for the pathToNames criterion.
		''' <p>
		''' This constraint is useful when building a certification path forward
		''' (from the target toward the trust anchor. If a partial path has been
		''' built, any candidate certificate must not include name constraints that
		''' would prohibit building a path to any of the names in the partial path.
		''' <p>
		''' The {@code names} parameter (if not {@code null}) is a
		''' {@code Collection} with one
		''' entry for each name to be included in the pathToNames
		''' criterion. Each entry is a {@code List} whose first entry is an
		''' {@code Integer} (the name type, 0-8) and whose second
		''' entry is a {@code String} or a byte array (the name, in
		''' string or ASN.1 DER encoded form, respectively).
		''' There can be multiple names of the same type. If {@code null}
		''' is supplied as the value for this argument, no
		''' pathToNames check will be performed.
		''' <p>
		''' Each name in the {@code Collection}
		''' may be specified either as a {@code String} or as an ASN.1 encoded
		''' byte array. For more details about the formats used, see
		''' {@link #addPathToName(int type, String name)
		''' addPathToName(int type, String name)} and
		''' {@link #addPathToName(int type, byte [] name)
		''' addPathToName(int type, byte [] name)}.
		''' <p>
		''' <strong>Note:</strong> for distinguished names, specify the byte
		''' array form instead of the String form. See the note in
		''' <seealso cref="#addPathToName(int, String)"/> for more information.
		''' <p>
		''' Note that the {@code names} parameter can contain duplicate
		''' names (same name and name type), but they may be removed from the
		''' {@code Collection} of names returned by the
		''' <seealso cref="#getPathToNames getPathToNames"/> method.
		''' <p>
		''' Note that a deep copy is performed on the {@code Collection} to
		''' protect against subsequent modifications.
		''' </summary>
		''' <param name="names"> a {@code Collection} with one entry per name
		'''              (or {@code null}) </param>
		''' <exception cref="IOException"> if a parsing error occurs </exception>
		''' <seealso cref= #getPathToNames </seealso>
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
        Public Overridable Sub setPathToNames(Of T1)(ByVal names As Collection(Of T1)) 'JavaToDotNetTempPropertySetpathToNames
		Public Overridable Property pathToNames(Of T1) As Collection(Of T1)
			Set(ByVal names As Collection(Of T1))
				If (names Is Nothing) OrElse names.empty Then
					pathToNames = Nothing
					pathToGeneralNames = Nothing
				Else
	'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
					Dim tempNames As [Set](Of List(Of ?)) = cloneAndCheckNames(names)
					pathToGeneralNames = parseNames(tempNames)
					' Ensure that we either set both of these or neither
					pathToNames = tempNames
				End If
			End Set
			Get
		End Property

		' called from CertPathHelper
		Friend Overridable Property pathToNamesInternal As [Set](Of GeneralNameInterface)
			Set(ByVal names As [Set](Of GeneralNameInterface))
				' set names to non-null dummy value
				' this breaks getPathToNames()
	'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				pathToNames = Collections.emptySet(Of List(Of ?))()
				pathToGeneralNames = names
			End Set
		End Property

		''' <summary>
		''' Adds a name to the pathToNames criterion. The {@code X509Certificate}
		''' must not include name constraints that would prohibit building a
		''' path to the specified name.
		''' <p>
		''' This method allows the caller to add a name to the set of names which
		''' the {@code X509Certificates}'s name constraints must permit.
		''' The specified name is added to any previous value for the
		''' pathToNames criterion.  If the name is a duplicate, it may be ignored.
		''' <p>
		''' The name is provided in string format. RFC 822, DNS, and URI names
		''' use the well-established string formats for those types (subject to
		''' the restrictions included in RFC 3280). IPv4 address names are
		''' supplied using dotted quad notation. OID address names are represented
		''' as a series of nonnegative integers separated by periods. And
		''' directory names (distinguished names) are supplied in RFC 2253 format.
		''' No standard string format is defined for otherNames, X.400 names,
		''' EDI party names, IPv6 address names, or any other type of names. They
		''' should be specified using the
		''' {@link #addPathToName(int type, byte [] name)
		''' addPathToName(int type, byte [] name)} method.
		''' <p>
		''' <strong>Note:</strong> for distinguished names, use
		''' <seealso cref="#addPathToName(int, byte[])"/> instead.
		''' This method should not be relied on as it can fail to match some
		''' certificates because of a loss of encoding information in the RFC 2253
		''' String form of some distinguished names.
		''' </summary>
		''' <param name="type"> the name type (0-8, as specified in
		'''             RFC 3280, section 4.2.1.7) </param>
		''' <param name="name"> the name in string form </param>
		''' <exception cref="IOException"> if a parsing error occurs </exception>
		Public Overridable Sub addPathToName(ByVal type As Integer, ByVal name As String)
			addPathToNameInternal(type, name)
		End Sub

		''' <summary>
		''' Adds a name to the pathToNames criterion. The {@code X509Certificate}
		''' must not include name constraints that would prohibit building a
		''' path to the specified name.
		''' <p>
		''' This method allows the caller to add a name to the set of names which
		''' the {@code X509Certificates}'s name constraints must permit.
		''' The specified name is added to any previous value for the
		''' pathToNames criterion. If the name is a duplicate, it may be ignored.
		''' <p>
		''' The name is provided as a byte array. This byte array should contain
		''' the DER encoded name, as it would appear in the GeneralName structure
		''' defined in RFC 3280 and X.509. The ASN.1 definition of this structure
		''' appears in the documentation for
		''' {@link #addSubjectAlternativeName(int type, byte [] name)
		''' addSubjectAlternativeName(int type, byte [] name)}.
		''' <p>
		''' Note that the byte array supplied here is cloned to protect against
		''' subsequent modifications.
		''' </summary>
		''' <param name="type"> the name type (0-8, as specified in
		'''             RFC 3280, section 4.2.1.7) </param>
		''' <param name="name"> a byte array containing the name in ASN.1 DER encoded form </param>
		''' <exception cref="IOException"> if a parsing error occurs </exception>
		Public Overridable Sub addPathToName(ByVal type As Integer, ByVal name As SByte ())
			' clone because byte arrays are modifiable
			addPathToNameInternal(type, name.clone())
		End Sub

		''' <summary>
		''' A private method that adds a name (String or byte array) to the
		''' pathToNames criterion. The {@code X509Certificate} must contain
		''' the specified pathToName.
		''' </summary>
		''' <param name="type"> the name type (0-8, as specified in
		'''             RFC 3280, section 4.2.1.7) </param>
		''' <param name="name"> the name in string or byte array form </param>
		''' <exception cref="IOException"> if an encoding error occurs (incorrect form for DN) </exception>
		Private Sub addPathToNameInternal(ByVal type As Integer, ByVal name As Object)
			' First, ensure that the name parses
			Dim tempName As GeneralNameInterface = makeGeneralNameInterface(type, name)
			If pathToGeneralNames Is Nothing Then
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				pathToNames = New HashSet(Of List(Of ?))
				pathToGeneralNames = New HashSet(Of GeneralNameInterface)
			End If
			Dim list As List(Of Object) = New List(Of Object)(2)
			list.add(Convert.ToInt32(type))
			list.add(name)
			pathToNames.add(list)
			pathToGeneralNames.add(tempName)
		End Sub

			Return x509Cert
		End Function

			Return serialNumber
		End Function

			Return issuer
		End Function

		''' <summary>
		''' <strong>Denigrated</strong>, use <seealso cref="#getIssuer()"/> or
		''' <seealso cref="#getIssuerAsBytes()"/> instead. This method should not be
		''' relied on as it can fail to match some certificates because of a loss of
		''' encoding information in the RFC 2253 String form of some distinguished
		''' names.
		''' <p>
		''' Returns the issuer criterion as a {@code String}. This
		''' distinguished name must match the issuer distinguished name in the
		''' {@code X509Certificate}. If {@code null}, the issuer criterion
		''' is disabled and any issuer distinguished name will do.
		''' <p>
		''' If the value returned is not {@code null}, it is a
		''' distinguished name, in RFC 2253 format.
		''' </summary>
		''' <returns> the required issuer distinguished name in RFC 2253 format
		'''         (or {@code null}) </returns>
		Public Overridable Property issuerAsString As String
			Get
				Return (If(issuer Is Nothing, Nothing, issuer.name))
			End Get
		End Property

		''' <summary>
		''' Returns the issuer criterion as a byte array. This distinguished name
		''' must match the issuer distinguished name in the
		''' {@code X509Certificate}. If {@code null}, the issuer criterion
		''' is disabled and any issuer distinguished name will do.
		''' <p>
		''' If the value returned is not {@code null}, it is a byte
		''' array containing a single DER encoded distinguished name, as defined in
		''' X.501. The ASN.1 notation for this structure is supplied in the
		''' documentation for
		''' <seealso cref="#setIssuer(byte [] issuerDN) setIssuer(byte [] issuerDN)"/>.
		''' <p>
		''' Note that the byte array returned is cloned to protect against
		''' subsequent modifications.
		''' </summary>
		''' <returns> a byte array containing the required issuer distinguished name
		'''         in ASN.1 DER format (or {@code null}) </returns>
		''' <exception cref="IOException"> if an encoding error occurs </exception>
		Public Overridable Property issuerAsBytes As SByte()
			Get
				Return (If(issuer Is Nothing, Nothing, issuer.encoded))
			End Get
		End Property

			Return subject
		End Function

		''' <summary>
		''' <strong>Denigrated</strong>, use <seealso cref="#getSubject()"/> or
		''' <seealso cref="#getSubjectAsBytes()"/> instead. This method should not be
		''' relied on as it can fail to match some certificates because of a loss of
		''' encoding information in the RFC 2253 String form of some distinguished
		''' names.
		''' <p>
		''' Returns the subject criterion as a {@code String}. This
		''' distinguished name must match the subject distinguished name in the
		''' {@code X509Certificate}. If {@code null}, the subject criterion
		''' is disabled and any subject distinguished name will do.
		''' <p>
		''' If the value returned is not {@code null}, it is a
		''' distinguished name, in RFC 2253 format.
		''' </summary>
		''' <returns> the required subject distinguished name in RFC 2253 format
		'''         (or {@code null}) </returns>
		Public Overridable Property subjectAsString As String
			Get
				Return (If(subject Is Nothing, Nothing, subject.name))
			End Get
		End Property

		''' <summary>
		''' Returns the subject criterion as a byte array. This distinguished name
		''' must match the subject distinguished name in the
		''' {@code X509Certificate}. If {@code null}, the subject criterion
		''' is disabled and any subject distinguished name will do.
		''' <p>
		''' If the value returned is not {@code null}, it is a byte
		''' array containing a single DER encoded distinguished name, as defined in
		''' X.501. The ASN.1 notation for this structure is supplied in the
		''' documentation for
		''' <seealso cref="#setSubject(byte [] subjectDN) setSubject(byte [] subjectDN)"/>.
		''' <p>
		''' Note that the byte array returned is cloned to protect against
		''' subsequent modifications.
		''' </summary>
		''' <returns> a byte array containing the required subject distinguished name
		'''         in ASN.1 DER format (or {@code null}) </returns>
		''' <exception cref="IOException"> if an encoding error occurs </exception>
		Public Overridable Property subjectAsBytes As SByte()
			Get
				Return (If(subject Is Nothing, Nothing, subject.encoded))
			End Get
		End Property

			If subjectKeyID Is Nothing Then Return Nothing
			Return subjectKeyID.clone()
		End Function

			If authorityKeyID Is Nothing Then Return Nothing
			Return authorityKeyID.clone()
		End Function

			If certificateValid Is Nothing Then Return Nothing
			Return CDate(certificateValid.clone())
		End Function

			If privateKeyValid Is Nothing Then Return Nothing
			Return CDate(privateKeyValid.clone())
		End Function

			If subjectPublicKeyAlgID Is Nothing Then Return Nothing
			Return subjectPublicKeyAlgID.ToString()
		End Function

			Return subjectPublicKey
		End Function

			If keyUsage Is Nothing Then Return Nothing
			Return keyUsage.clone()
		End Function

			Return keyPurposeSet
		End Function

			Return matchAllSubjectAltNames
		End Function

		''' <summary>
		''' Returns a copy of the subjectAlternativeNames criterion.
		''' The {@code X509Certificate} must contain all or at least one
		''' of the specified subjectAlternativeNames, depending on the value
		''' of the matchAllNames flag (see {@link #getMatchAllSubjectAltNames
		''' getMatchAllSubjectAltNames}). If the value returned is
		''' {@code null}, no subjectAlternativeNames check will be performed.
		''' <p>
		''' If the value returned is not {@code null}, it is a
		''' {@code Collection} with
		''' one entry for each name to be included in the subject alternative name
		''' criterion. Each entry is a {@code List} whose first entry is an
		''' {@code Integer} (the name type, 0-8) and whose second
		''' entry is a {@code String} or a byte array (the name, in
		''' string or ASN.1 DER encoded form, respectively).
		''' There can be multiple names of the same type.  Note that the
		''' {@code Collection} returned may contain duplicate names (same name
		''' and name type).
		''' <p>
		''' Each subject alternative name in the {@code Collection}
		''' may be specified either as a {@code String} or as an ASN.1 encoded
		''' byte array. For more details about the formats used, see
		''' {@link #addSubjectAlternativeName(int type, String name)
		''' addSubjectAlternativeName(int type, String name)} and
		''' {@link #addSubjectAlternativeName(int type, byte [] name)
		''' addSubjectAlternativeName(int type, byte [] name)}.
		''' <p>
		''' Note that a deep copy is performed on the {@code Collection} to
		''' protect against subsequent modifications.
		''' </summary>
		''' <returns> a {@code Collection} of names (or {@code null}) </returns>
		''' <seealso cref= #setSubjectAlternativeNames </seealso>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			If subjectAlternativeNames Is Nothing Then Return Nothing
			Return cloneNames(subjectAlternativeNames)
		End Function

		''' <summary>
		''' Clone an object of the form passed to
		''' setSubjectAlternativeNames and setPathToNames.
		''' Throw a {@code RuntimeException} if the argument is malformed.
		''' <p>
		''' This method wraps cloneAndCheckNames, changing any
		''' {@code IOException} into a {@code RuntimeException}. This
		''' method should be used when the object being
		''' cloned has already been checked, so there should never be any exceptions.
		''' </summary>
		''' <param name="names"> a {@code Collection} with one entry per name.
		'''              Each entry is a {@code List} whose first entry
		'''              is an Integer (the name type, 0-8) and whose second
		'''              entry is a String or a byte array (the name, in
		'''              string or ASN.1 DER encoded form, respectively).
		'''              There can be multiple names of the same type. Null
		'''              is not an acceptable value. </param>
		''' <returns> a deep copy of the specified {@code Collection} </returns>
		''' <exception cref="RuntimeException"> if a parsing error occurs </exception>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Private Shared Function cloneNames(Of T1)(ByVal names As Collection(Of T1)) As [Set](Of List(Of ?))
			Try
				Return cloneAndCheckNames(names)
			Catch e As java.io.IOException
				Throw New RuntimeException("cloneNames encountered IOException: " & e.Message)
			End Try
		End Function

		''' <summary>
		''' Clone and check an argument of the form passed to
		''' setSubjectAlternativeNames and setPathToNames.
		''' Throw an {@code IOException} if the argument is malformed.
		''' </summary>
		''' <param name="names"> a {@code Collection} with one entry per name.
		'''              Each entry is a {@code List} whose first entry
		'''              is an Integer (the name type, 0-8) and whose second
		'''              entry is a String or a byte array (the name, in
		'''              string or ASN.1 DER encoded form, respectively).
		'''              There can be multiple names of the same type.
		'''              {@code null} is not an acceptable value. </param>
		''' <returns> a deep copy of the specified {@code Collection} </returns>
		''' <exception cref="IOException"> if a parsing error occurs </exception>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Private Shared Function cloneAndCheckNames(Of T1)(ByVal names As Collection(Of T1)) As [Set](Of List(Of ?))
			' Copy the Lists and Collection
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim namesCopy As [Set](Of List(Of ?)) = New HashSet(Of List(Of ?))
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			For Each o As List(Of ?) In names
				namesCopy.add(New List(Of Object)(o))
			Next o

			' Check the contents of the Lists and clone any byte arrays
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			For Each list As List(Of ?) In namesCopy
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
				Dim nameList As List(Of Object) = CType(list, List(Of Object)) ' See javadoc for parameter "names".
				If nameList.size() <> 2 Then Throw New java.io.IOException("name list size not 2")
				Dim o As Object = nameList.get(0)
				If Not(TypeOf o Is Integer?) Then Throw New java.io.IOException("expected an Integer")
				Dim nameType As Integer = CInt(Fix(o))
				If (nameType < 0) OrElse (nameType > 8) Then Throw New java.io.IOException("name type not 0-8")
				Dim nameObject As Object = nameList.get(1)
				If Not(TypeOf nameObject Is SByte()) AndAlso Not(TypeOf nameObject Is String) Then
					If debug IsNot Nothing Then debug.println("X509CertSelector.cloneAndCheckNames() " & "name not byte array")
					Throw New java.io.IOException("name not byte array or String")
				End If
				If TypeOf nameObject Is SByte() Then nameList.set(1, CType(nameObject, SByte()).clone())
			Next list
			Return namesCopy
		End Function

			If ncBytes Is Nothing Then
				Return Nothing
			Else
				Return ncBytes.clone()
			End If
		End Function

			Return basicConstraints
		End Function

			Return policySet
		End Function

		''' <summary>
		''' Returns a copy of the pathToNames criterion. The
		''' {@code X509Certificate} must not include name constraints that would
		''' prohibit building a path to the specified names. If the value
		''' returned is {@code null}, no pathToNames check will be performed.
		''' <p>
		''' If the value returned is not {@code null}, it is a
		''' {@code Collection} with one
		''' entry for each name to be included in the pathToNames
		''' criterion. Each entry is a {@code List} whose first entry is an
		''' {@code Integer} (the name type, 0-8) and whose second
		''' entry is a {@code String} or a byte array (the name, in
		''' string or ASN.1 DER encoded form, respectively).
		''' There can be multiple names of the same type. Note that the
		''' {@code Collection} returned may contain duplicate names (same
		''' name and name type).
		''' <p>
		''' Each name in the {@code Collection}
		''' may be specified either as a {@code String} or as an ASN.1 encoded
		''' byte array. For more details about the formats used, see
		''' {@link #addPathToName(int type, String name)
		''' addPathToName(int type, String name)} and
		''' {@link #addPathToName(int type, byte [] name)
		''' addPathToName(int type, byte [] name)}.
		''' <p>
		''' Note that a deep copy is performed on the {@code Collection} to
		''' protect against subsequent modifications.
		''' </summary>
		''' <returns> a {@code Collection} of names (or {@code null}) </returns>
		''' <seealso cref= #setPathToNames </seealso>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			If pathToNames Is Nothing Then Return Nothing
			Return cloneNames(pathToNames)
		End Function

		''' <summary>
		''' Return a printable representation of the {@code CertSelector}.
		''' </summary>
		''' <returns> a {@code String} describing the contents of the
		'''         {@code CertSelector} </returns>
		Public Overrides Function ToString() As String
			Dim sb As New StringBuffer
			sb.append("X509CertSelector: [" & vbLf)
			If x509Cert IsNot Nothing Then sb.append("  Certificate: " & x509Cert.ToString() & vbLf)
			If serialNumber IsNot Nothing Then sb.append("  Serial Number: " & serialNumber.ToString() & vbLf)
			If issuer IsNot Nothing Then sb.append("  Issuer: " & issuerAsString & vbLf)
			If subject IsNot Nothing Then sb.append("  Subject: " & subjectAsString & vbLf)
			sb.append("  matchAllSubjectAltNames flag: " & Convert.ToString(matchAllSubjectAltNames) & vbLf)
			If subjectAlternativeNames IsNot Nothing Then
				sb.append("  SubjectAlternativeNames:" & vbLf)
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim i As [Iterator](Of List(Of ?)) = subjectAlternativeNames.GetEnumerator()
				Do While i.MoveNext()
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
					Dim list As List(Of ?) = i.Current
					sb.append("    type " & list.get(0) & ", name " & list.get(1) & vbLf)
				Loop
			End If
			If subjectKeyID IsNot Nothing Then
				Dim enc As New sun.misc.HexDumpEncoder
				sb.append("  Subject Key Identifier: " & enc.encodeBuffer(subjectKeyID) & vbLf)
			End If
			If authorityKeyID IsNot Nothing Then
				Dim enc As New sun.misc.HexDumpEncoder
				sb.append("  Authority Key Identifier: " & enc.encodeBuffer(authorityKeyID) & vbLf)
			End If
			If certificateValid IsNot Nothing Then sb.append("  Certificate Valid: " & certificateValid.ToString() & vbLf)
			If privateKeyValid IsNot Nothing Then sb.append("  Private Key Valid: " & privateKeyValid.ToString() & vbLf)
			If subjectPublicKeyAlgID IsNot Nothing Then sb.append("  Subject Public Key AlgID: " & subjectPublicKeyAlgID.ToString() & vbLf)
			If subjectPublicKey IsNot Nothing Then sb.append("  Subject Public Key: " & subjectPublicKey.ToString() & vbLf)
			If keyUsage IsNot Nothing Then sb.append("  Key Usage: " & keyUsageToString(keyUsage) & vbLf)
			If keyPurposeSet IsNot Nothing Then sb.append("  Extended Key Usage: " & keyPurposeSet.ToString() & vbLf)
			If policy_Renamed IsNot Nothing Then sb.append("  Policy: " & policy_Renamed.ToString() & vbLf)
			If pathToGeneralNames IsNot Nothing Then
				sb.append("  Path to names:" & vbLf)
				Dim i As [Iterator](Of GeneralNameInterface) = pathToGeneralNames.GetEnumerator()
				Do While i.MoveNext()
					sb.append("    " & i.Current & vbLf)
				Loop
			End If
			sb.append("]")
			Return sb.ToString()
		End Function

		' Copied from sun.security.x509.KeyUsageExtension
		' (without calling the superclass)
		''' <summary>
		''' Returns a printable representation of the KeyUsage.
		''' </summary>
		Private Shared Function keyUsageToString(ByVal k As Boolean()) As String
			Dim s As String = "KeyUsage [" & vbLf
			Try
				If k(0) Then s &= "  DigitalSignature" & vbLf
				If k(1) Then s &= "  Non_repudiation" & vbLf
				If k(2) Then s &= "  Key_Encipherment" & vbLf
				If k(3) Then s &= "  Data_Encipherment" & vbLf
				If k(4) Then s &= "  Key_Agreement" & vbLf
				If k(5) Then s &= "  Key_CertSign" & vbLf
				If k(6) Then s &= "  Crl_Sign" & vbLf
				If k(7) Then s &= "  Encipher_Only" & vbLf
				If k(8) Then s &= "  Decipher_Only" & vbLf
			Catch ex As ArrayIndexOutOfBoundsException
			End Try

			s &= "]" & vbLf

			Return (s)
		End Function

		''' <summary>
		''' Returns an Extension object given any X509Certificate and extension oid.
		''' Throw an {@code IOException} if the extension byte value is
		''' malformed.
		''' </summary>
		''' <param name="cert"> a {@code X509Certificate} </param>
		''' <param name="extId"> an {@code integer} which specifies the extension index.
		''' Currently, the supported extensions are as follows:
		''' index 0 - PrivateKeyUsageExtension
		''' index 1 - SubjectAlternativeNameExtension
		''' index 2 - NameConstraintsExtension
		''' index 3 - CertificatePoliciesExtension
		''' index 4 - ExtendedKeyUsageExtension </param>
		''' <returns> an {@code Extension} object whose real type is as specified
		''' by the extension oid. </returns>
		''' <exception cref="IOException"> if cannot construct the {@code Extension}
		''' object with the extension encoding retrieved from the passed in
		''' {@code X509Certificate}. </exception>
		Private Shared Function getExtensionObject(ByVal cert As X509Certificate, ByVal extId As Integer) As Extension
			If TypeOf cert Is X509CertImpl Then
				Dim impl As X509CertImpl = CType(cert, X509CertImpl)
				Select Case extId
				Case PRIVATE_KEY_USAGE_ID
					Return impl.privateKeyUsageExtension
				Case SUBJECT_ALT_NAME_ID
					Return impl.subjectAlternativeNameExtension
				Case NAME_CONSTRAINTS_ID
					Return impl.nameConstraintsExtension
				Case CERT_POLICIES_ID
					Return impl.certificatePoliciesExtension
				Case EXTENDED_KEY_USAGE_ID
					Return impl.extendedKeyUsageExtension
				Case Else
					Return Nothing
				End Select
			End If
			Dim rawExtVal As SByte() = cert.getExtensionValue(EXTENSION_OIDS(extId))
			If rawExtVal Is Nothing Then Return Nothing
			Dim [in] As New sun.security.util.DerInputStream(rawExtVal)
			Dim encoded As SByte() = [in].octetString
			Select Case extId
			Case PRIVATE_KEY_USAGE_ID
				Try
					Return New PrivateKeyUsageExtension([FALSE], encoded)
				Catch ex As CertificateException
					Throw New java.io.IOException(ex.Message)
				End Try
			Case SUBJECT_ALT_NAME_ID
				Return New SubjectAlternativeNameExtension([FALSE], encoded)
			Case NAME_CONSTRAINTS_ID
				Return New NameConstraintsExtension([FALSE], encoded)
			Case CERT_POLICIES_ID
				Return New CertificatePoliciesExtension([FALSE], encoded)
			Case EXTENDED_KEY_USAGE_ID
				Return New ExtendedKeyUsageExtension([FALSE], encoded)
			Case Else
				Return Nothing
			End Select
		End Function

		''' <summary>
		''' Decides whether a {@code Certificate} should be selected.
		''' </summary>
		''' <param name="cert"> the {@code Certificate} to be checked </param>
		''' <returns> {@code true} if the {@code Certificate} should be
		'''         selected, {@code false} otherwise </returns>
		Public Overridable Function match(ByVal cert As Certificate) As Boolean Implements CertSelector.match
			If Not(TypeOf cert Is X509Certificate) Then Return False
			Dim xcert As X509Certificate = CType(cert, X509Certificate)

			If debug IsNot Nothing Then debug.println("X509CertSelector.match(SN: " & (xcert.serialNumber).ToString(16) & vbLf & "  Issuer: " & xcert.issuerDN & vbLf & "  Subject: " & xcert.subjectDN & ")")

			' match on X509Certificate 
			If x509Cert IsNot Nothing Then
				If Not x509Cert.Equals(xcert) Then
					If debug IsNot Nothing Then debug.println("X509CertSelector.match: " & "certs don't match")
					Return False
				End If
			End If

			' match on serial number 
			If serialNumber IsNot Nothing Then
				If Not serialNumber.Equals(xcert.serialNumber) Then
					If debug IsNot Nothing Then debug.println("X509CertSelector.match: " & "serial numbers don't match")
					Return False
				End If
			End If

			' match on issuer name 
			If issuer IsNot Nothing Then
				If Not issuer.Equals(xcert.issuerX500Principal) Then
					If debug IsNot Nothing Then debug.println("X509CertSelector.match: " & "issuer DNs don't match")
					Return False
				End If
			End If

			' match on subject name 
			If subject IsNot Nothing Then
				If Not subject.Equals(xcert.subjectX500Principal) Then
					If debug IsNot Nothing Then debug.println("X509CertSelector.match: " & "subject DNs don't match")
					Return False
				End If
			End If

			' match on certificate validity range 
			If certificateValid IsNot Nothing Then
				Try
					xcert.checkValidity(certificateValid)
				Catch e As CertificateException
					If debug IsNot Nothing Then debug.println("X509CertSelector.match: " & "certificate not within validity period")
					Return False
				End Try
			End If

			' match on subject public key 
			If subjectPublicKeyBytes IsNot Nothing Then
				Dim certKey As SByte() = xcert.publicKey.encoded
				If Not Array.Equals(subjectPublicKeyBytes, certKey) Then
					If debug IsNot Nothing Then debug.println("X509CertSelector.match: " & "subject public keys don't match")
					Return False
				End If
			End If

			Dim result As Boolean = matchBasicConstraints(xcert) AndAlso matchKeyUsage(xcert) AndAlso matchExtendedKeyUsage(xcert) AndAlso matchSubjectKeyID(xcert) AndAlso matchAuthorityKeyID(xcert) AndAlso matchPrivateKeyValid(xcert) AndAlso matchSubjectPublicKeyAlgID(xcert) AndAlso matchPolicy(xcert) AndAlso matchSubjectAlternativeNames(xcert) AndAlso matchPathToNames(xcert) AndAlso matchNameConstraints(xcert)

			If result AndAlso (debug IsNot Nothing) Then debug.println("X509CertSelector.match returning: true")
			Return result
		End Function

		' match on subject key identifier extension value 
		Private Function matchSubjectKeyID(ByVal xcert As X509Certificate) As Boolean
			If subjectKeyID Is Nothing Then Return True
			Try
				Dim extVal As SByte() = xcert.getExtensionValue("2.5.29.14")
				If extVal Is Nothing Then
					If debug IsNot Nothing Then debug.println("X509CertSelector.match: " & "no subject key ID extension")
					Return False
				End If
				Dim [in] As New sun.security.util.DerInputStream(extVal)
				Dim certSubjectKeyID As SByte() = [in].octetString
				If certSubjectKeyID Is Nothing OrElse (Not Array.Equals(subjectKeyID, certSubjectKeyID)) Then
					If debug IsNot Nothing Then debug.println("X509CertSelector.match: " & "subject key IDs don't match")
					Return False
				End If
			Catch ex As java.io.IOException
				If debug IsNot Nothing Then debug.println("X509CertSelector.match: " & "exception in subject key ID check")
				Return False
			End Try
			Return True
		End Function

		' match on authority key identifier extension value 
		Private Function matchAuthorityKeyID(ByVal xcert As X509Certificate) As Boolean
			If authorityKeyID Is Nothing Then Return True
			Try
				Dim extVal As SByte() = xcert.getExtensionValue("2.5.29.35")
				If extVal Is Nothing Then
					If debug IsNot Nothing Then debug.println("X509CertSelector.match: " & "no authority key ID extension")
					Return False
				End If
				Dim [in] As New sun.security.util.DerInputStream(extVal)
				Dim certAuthKeyID As SByte() = [in].octetString
				If certAuthKeyID Is Nothing OrElse (Not Array.Equals(authorityKeyID, certAuthKeyID)) Then
					If debug IsNot Nothing Then debug.println("X509CertSelector.match: " & "authority key IDs don't match")
					Return False
				End If
			Catch ex As java.io.IOException
				If debug IsNot Nothing Then debug.println("X509CertSelector.match: " & "exception in authority key ID check")
				Return False
			End Try
			Return True
		End Function

		' match on private key usage range 
		Private Function matchPrivateKeyValid(ByVal xcert As X509Certificate) As Boolean
			If privateKeyValid Is Nothing Then Return True
			Dim ext As PrivateKeyUsageExtension = Nothing
			Try
				ext = CType(getExtensionObject(xcert, PRIVATE_KEY_USAGE_ID), PrivateKeyUsageExtension)
				If ext IsNot Nothing Then ext.valid(privateKeyValid)
			Catch e1 As CertificateExpiredException
				If debug IsNot Nothing Then
					Dim time As String = "n/a"
					Try
						Dim notAfter As Date = ext.get(PrivateKeyUsageExtension.NOT_AFTER)
						time = notAfter.ToString()
					Catch ex As CertificateException
						' not able to retrieve notAfter value
					End Try
					debug.println("X509CertSelector.match: private key usage not " & "within validity date; ext.NOT_After: " & time & "; X509CertSelector: " & Me.ToString())
					Console.WriteLine(e1.ToString())
					Console.Write(e1.StackTrace)
				End If
				Return False
			Catch e2 As CertificateNotYetValidException
				If debug IsNot Nothing Then
					Dim time As String = "n/a"
					Try
						Dim notBefore As Date = ext.get(PrivateKeyUsageExtension.NOT_BEFORE)
						time = notBefore.ToString()
					Catch ex As CertificateException
						' not able to retrieve notBefore value
					End Try
					debug.println("X509CertSelector.match: private key usage not " & "within validity date; ext.NOT_BEFORE: " & time & "; X509CertSelector: " & Me.ToString())
					Console.WriteLine(e2.ToString())
					Console.Write(e2.StackTrace)
				End If
				Return False
			Catch e4 As java.io.IOException
				If debug IsNot Nothing Then
					debug.println("X509CertSelector.match: IOException in " & "private key usage check; X509CertSelector: " & Me.ToString())
					Console.WriteLine(e4.ToString())
					Console.Write(e4.StackTrace)
				End If
				Return False
			End Try
			Return True
		End Function

		' match on subject public key algorithm OID 
		Private Function matchSubjectPublicKeyAlgID(ByVal xcert As X509Certificate) As Boolean
			If subjectPublicKeyAlgID Is Nothing Then Return True
			Try
				Dim encodedKey As SByte() = xcert.publicKey.encoded
				Dim val As New sun.security.util.DerValue(encodedKey)
				If val.tag <> sun.security.util.DerValue.tag_Sequence Then Throw New java.io.IOException("invalid key format")

				Dim algID As AlgorithmId = AlgorithmId.parse(val.data.derValue)
				If debug IsNot Nothing Then debug.println("X509CertSelector.match: subjectPublicKeyAlgID = " & subjectPublicKeyAlgID & ", xcert subjectPublicKeyAlgID = " & algID.oID)
				If Not subjectPublicKeyAlgID.Equals(CObj(algID.oID)) Then
					If debug IsNot Nothing Then debug.println("X509CertSelector.match: " & "subject public key alg IDs don't match")
					Return False
				End If
			Catch e5 As java.io.IOException
				If debug IsNot Nothing Then debug.println("X509CertSelector.match: IOException in subject " & "public key algorithm OID check")
				Return False
			End Try
			Return True
		End Function

		' match on key usage extension value 
		Private Function matchKeyUsage(ByVal xcert As X509Certificate) As Boolean
			If keyUsage Is Nothing Then Return True
			Dim certKeyUsage As Boolean() = xcert.keyUsage
			If certKeyUsage IsNot Nothing Then
				For keyBit As Integer = 0 To keyUsage.Length - 1
					If keyUsage(keyBit) AndAlso ((keyBit >= certKeyUsage.Length) OrElse (Not certKeyUsage(keyBit))) Then
						If debug IsNot Nothing Then debug.println("X509CertSelector.match: " & "key usage bits don't match")
						Return False
					End If
				Next keyBit
			End If
			Return True
		End Function

		' match on extended key usage purpose OIDs 
		Private Function matchExtendedKeyUsage(ByVal xcert As X509Certificate) As Boolean
			If (keyPurposeSet Is Nothing) OrElse keyPurposeSet.empty Then Return True
			Try
				Dim ext As ExtendedKeyUsageExtension = CType(getExtensionObject(xcert, EXTENDED_KEY_USAGE_ID), ExtendedKeyUsageExtension)
				If ext IsNot Nothing Then
					Dim certKeyPurposeVector As Vector(Of sun.security.util.ObjectIdentifier) = ext.get(ExtendedKeyUsageExtension.USAGES)
					If (Not certKeyPurposeVector.contains(ANY_EXTENDED_KEY_USAGE)) AndAlso (Not certKeyPurposeVector.containsAll(keyPurposeOIDSet)) Then
						If debug IsNot Nothing Then debug.println("X509CertSelector.match: cert failed " & "extendedKeyUsage criterion")
						Return False
					End If
				End If
			Catch ex As java.io.IOException
				If debug IsNot Nothing Then debug.println("X509CertSelector.match: " & "IOException in extended key usage check")
				Return False
			End Try
			Return True
		End Function

		' match on subject alternative name extension names 
		Private Function matchSubjectAlternativeNames(ByVal xcert As X509Certificate) As Boolean
			If (subjectAlternativeNames Is Nothing) OrElse subjectAlternativeNames.empty Then Return True
			Try
				Dim sanExt As SubjectAlternativeNameExtension = CType(getExtensionObject(xcert, SUBJECT_ALT_NAME_ID), SubjectAlternativeNameExtension)
				If sanExt Is Nothing Then
					If debug IsNot Nothing Then debug.println("X509CertSelector.match: " & "no subject alternative name extension")
					Return False
				End If
				Dim certNames As GeneralNames = sanExt.get(SubjectAlternativeNameExtension.SUBJECT_NAME)
				Dim i As [Iterator](Of GeneralNameInterface) = subjectAlternativeGeneralNames.GetEnumerator()
				Do While i.MoveNext()
					Dim matchName As GeneralNameInterface = i.Current
					Dim found As Boolean = False
					Dim t As [Iterator](Of GeneralName) = certNames.GetEnumerator()
					Do While t.hasNext() AndAlso Not found
						Dim certName As GeneralNameInterface = (t.next()).name
						found = certName.Equals(matchName)
					Loop
					If (Not found) AndAlso (matchAllSubjectAltNames OrElse (Not i.hasNext())) Then
						If debug IsNot Nothing Then debug.println("X509CertSelector.match: subject alternative " & "name " & matchName & " not found")
						Return False
					ElseIf found AndAlso (Not matchAllSubjectAltNames) Then
						Exit Do
					End If
				Loop
			Catch ex As java.io.IOException
				If debug IsNot Nothing Then debug.println("X509CertSelector.match: IOException in subject " & "alternative name check")
				Return False
			End Try
			Return True
		End Function

		' match on name constraints 
		Private Function matchNameConstraints(ByVal xcert As X509Certificate) As Boolean
			If nc Is Nothing Then Return True
			Try
				If Not nc.verify(xcert) Then
					If debug IsNot Nothing Then debug.println("X509CertSelector.match: " & "name constraints not satisfied")
					Return False
				End If
			Catch e As java.io.IOException
				If debug IsNot Nothing Then debug.println("X509CertSelector.match: " & "IOException in name constraints check")
				Return False
			End Try
			Return True
		End Function

		' match on policy OIDs 
		Private Function matchPolicy(ByVal xcert As X509Certificate) As Boolean
			If policy_Renamed Is Nothing Then Return True
			Try
				Dim ext As CertificatePoliciesExtension = CType(getExtensionObject(xcert, CERT_POLICIES_ID), CertificatePoliciesExtension)
				If ext Is Nothing Then
					If debug IsNot Nothing Then debug.println("X509CertSelector.match: " & "no certificate policy extension")
					Return False
				End If
				Dim policies As List(Of PolicyInformation) = ext.get(CertificatePoliciesExtension.POLICIES)
	'            
	'             * Convert the Vector of PolicyInformation to a Vector
	'             * of CertificatePolicyIds for easier comparison.
	'             
				Dim policyIDs As List(Of CertificatePolicyId) = New List(Of CertificatePolicyId)(policies.size())
				For Each info As PolicyInformation In policies
					policyIDs.add(info.policyIdentifier)
				Next info
				If policy_Renamed IsNot Nothing Then
					Dim foundOne As Boolean = False
	'                
	'                 * if the user passes in an empty policy Set, then
	'                 * we just want to make sure that the candidate certificate
	'                 * has some policy OID in its CertPoliciesExtension
	'                 
					If policy_Renamed.certPolicyIds.empty Then
						If policyIDs.empty Then
							If debug IsNot Nothing Then debug.println("X509CertSelector.match: " & "cert failed policyAny criterion")
							Return False
						End If
					Else
						For Each id As CertificatePolicyId In policy_Renamed.certPolicyIds
							If policyIDs.contains(id) Then
								foundOne = True
								Exit For
							End If
						Next id
						If Not foundOne Then
							If debug IsNot Nothing Then debug.println("X509CertSelector.match: " & "cert failed policyAny criterion")
							Return False
						End If
					End If
				End If
			Catch ex As java.io.IOException
				If debug IsNot Nothing Then debug.println("X509CertSelector.match: " & "IOException in certificate policy ID check")
				Return False
			End Try
			Return True
		End Function

		' match on pathToNames 
		Private Function matchPathToNames(ByVal xcert As X509Certificate) As Boolean
			If pathToGeneralNames Is Nothing Then Return True
			Try
				Dim ext As NameConstraintsExtension = CType(getExtensionObject(xcert, NAME_CONSTRAINTS_ID), NameConstraintsExtension)
				If ext Is Nothing Then Return True
				If (debug IsNot Nothing) AndAlso sun.security.util.Debug.isOn("certpath") Then
					debug.println("X509CertSelector.match pathToNames:" & vbLf)
					Dim i As [Iterator](Of GeneralNameInterface) = pathToGeneralNames.GetEnumerator()
					Do While i.MoveNext()
						debug.println("    " & i.Current & vbLf)
					Loop
				End If

				Dim permitted As GeneralSubtrees = ext.get(NameConstraintsExtension.PERMITTED_SUBTREES)
				Dim excluded As GeneralSubtrees = ext.get(NameConstraintsExtension.EXCLUDED_SUBTREES)
				If excluded IsNot Nothing Then
					If matchExcluded(excluded) = False Then Return False
				End If
				If permitted IsNot Nothing Then
					If matchPermitted(permitted) = False Then Return False
				End If
			Catch ex As java.io.IOException
				If debug IsNot Nothing Then debug.println("X509CertSelector.match: " & "IOException in name constraints check")
				Return False
			End Try
			Return True
		End Function

		Private Function matchExcluded(ByVal excluded As GeneralSubtrees) As Boolean
	'        
	'         * Enumerate through excluded and compare each entry
	'         * to all pathToNames. If any pathToName is within any of the
	'         * subtrees listed in excluded, return false.
	'         
			Dim t As [Iterator](Of GeneralSubtree) = excluded.GetEnumerator()
			Do While t.MoveNext()
				Dim tree As GeneralSubtree = t.Current
				Dim excludedName As GeneralNameInterface = tree.name.name
				Dim i As [Iterator](Of GeneralNameInterface) = pathToGeneralNames.GetEnumerator()
				Do While i.MoveNext()
					Dim pathToName As GeneralNameInterface = i.Current
					If excludedName.type = pathToName.type Then
						Select Case pathToName.constrains(excludedName)
						Case GeneralNameInterface.NAME_WIDENS, GeneralNameInterface.NAME_MATCH
							If debug IsNot Nothing Then
								debug.println("X509CertSelector.match: name constraints " & "inhibit path to specified name")
								debug.println("X509CertSelector.match: excluded name: " & pathToName)
							End If
							Return False
						Case Else
						End Select
					End If
				Loop
			Loop
			Return True
		End Function

		Private Function matchPermitted(ByVal permitted As GeneralSubtrees) As Boolean
	'        
	'         * Enumerate through pathToNames, checking that each pathToName
	'         * is in at least one of the subtrees listed in permitted.
	'         * If not, return false. However, if no subtrees of a given type
	'         * are listed, all names of that type are permitted.
	'         
			Dim i As [Iterator](Of GeneralNameInterface) = pathToGeneralNames.GetEnumerator()
			Do While i.MoveNext()
				Dim pathToName As GeneralNameInterface = i.Current
				Dim t As [Iterator](Of GeneralSubtree) = permitted.GetEnumerator()
				Dim permittedNameFound As Boolean = False
				Dim nameTypeFound As Boolean = False
				Dim names As String = ""
				Do While t.hasNext() AndAlso Not permittedNameFound
					Dim tree As GeneralSubtree = t.next()
					Dim permittedName As GeneralNameInterface = tree.name.name
					If permittedName.type = pathToName.type Then
						nameTypeFound = True
						names = names & "  " & permittedName
						Select Case pathToName.constrains(permittedName)
						Case GeneralNameInterface.NAME_WIDENS, GeneralNameInterface.NAME_MATCH
							permittedNameFound = True
						Case Else
						End Select
					End If
				Loop
				If (Not permittedNameFound) AndAlso nameTypeFound Then
					If debug IsNot Nothing Then debug.println("X509CertSelector.match: " & "name constraints inhibit path to specified name; " & "permitted names of type " & pathToName.type & ": " & names)
					Return False
				End If
			Loop
			Return True
		End Function

		' match on basic constraints 
		Private Function matchBasicConstraints(ByVal xcert As X509Certificate) As Boolean
			If basicConstraints = -1 Then Return True
			Dim maxPathLen As Integer = xcert.basicConstraints
			If basicConstraints = -2 Then
				If maxPathLen <> -1 Then
					If debug IsNot Nothing Then debug.println("X509CertSelector.match: not an EE cert")
					Return False
				End If
			Else
				If maxPathLen < basicConstraints Then
					If debug IsNot Nothing Then debug.println("X509CertSelector.match: cert's maxPathLen " & "is less than the min maxPathLen set by " & "basicConstraints. " & "(" & maxPathLen & " < " & basicConstraints & ")")
					Return False
				End If
			End If
			Return True
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Private Shared Function cloneSet(Of T)(ByVal [set] As [Set](Of T)) As [Set](Of T) ' Safe casts assuming clone() works correctly
			If TypeOf [set] Is HashSet Then
				Dim clone As Object = CType([set], HashSet(Of T)).clone()
				Return CType(clone, Set(Of T))
			Else
				Return New HashSet(Of T)([set])
			End If
		End Function

		''' <summary>
		''' Returns a copy of this object.
		''' </summary>
		''' <returns> the copy </returns>
		Public Overridable Function clone() As Object Implements CertSelector.clone
			Try
				Dim copy As X509CertSelector = CType(MyBase.clone(), X509CertSelector)
				' Must clone these because addPathToName et al. modify them
				If subjectAlternativeNames IsNot Nothing Then
					copy.subjectAlternativeNames = cloneSet(subjectAlternativeNames)
					copy.subjectAlternativeGeneralNames = cloneSet(subjectAlternativeGeneralNames)
				End If
				If pathToGeneralNames IsNot Nothing Then
					copy.pathToNames = cloneSet(pathToNames)
					copy.pathToGeneralNames = cloneSet(pathToGeneralNames)
				End If
				Return copy
			Catch e As CloneNotSupportedException
				' Cannot happen 
				Throw New InternalError(e.ToString(), e)
			End Try
		End Function
	End Class

End Namespace