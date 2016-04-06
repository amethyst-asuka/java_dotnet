Imports System.Collections.Generic

'
' * Copyright (c) 2012, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' A {@code PKIXCertPathChecker} for checking the revocation status of
	''' certificates with the PKIX algorithm.
	''' 
	''' <p>A {@code PKIXRevocationChecker} checks the revocation status of
	''' certificates with the Online Certificate Status Protocol (OCSP) or
	''' Certificate Revocation Lists (CRLs). OCSP is described in RFC 2560 and
	''' is a network protocol for determining the status of a certificate. A CRL
	''' is a time-stamped list identifying revoked certificates, and RFC 5280
	''' describes an algorithm for determining the revocation status of certificates
	''' using CRLs.
	''' 
	''' <p>Each {@code PKIXRevocationChecker} must be able to check the revocation
	''' status of certificates with OCSP and CRLs. By default, OCSP is the
	''' preferred mechanism for checking revocation status, with CRLs as the
	''' fallback mechanism. However, this preference can be switched to CRLs with
	''' the <seealso cref="Option#PREFER_CRLS PREFER_CRLS"/> option. In addition, the fallback
	''' mechanism can be disabled with the <seealso cref="Option#NO_FALLBACK NO_FALLBACK"/>
	''' option.
	''' 
	''' <p>A {@code PKIXRevocationChecker} is obtained by calling the
	''' <seealso cref="CertPathValidator#getRevocationChecker getRevocationChecker"/> method
	''' of a PKIX {@code CertPathValidator}. Additional parameters and options
	''' specific to revocation can be set (by calling the
	''' <seealso cref="#setOcspResponder setOcspResponder"/> method for instance). The
	''' {@code PKIXRevocationChecker} is added to a {@code PKIXParameters} object
	''' using the <seealso cref="PKIXParameters#addCertPathChecker addCertPathChecker"/>
	''' or <seealso cref="PKIXParameters#setCertPathCheckers setCertPathCheckers"/> method,
	''' and then the {@code PKIXParameters} is passed along with the {@code CertPath}
	''' to be validated to the <seealso cref="CertPathValidator#validate validate"/> method
	''' of a PKIX {@code CertPathValidator}. When supplying a revocation checker in
	''' this manner, it will be used to check revocation irrespective of the setting
	''' of the <seealso cref="PKIXParameters#isRevocationEnabled RevocationEnabled"/> flag.
	''' Similarly, a {@code PKIXRevocationChecker} may be added to a
	''' {@code PKIXBuilderParameters} object for use with a PKIX
	''' {@code CertPathBuilder}.
	''' 
	''' <p>Note that when a {@code PKIXRevocationChecker} is added to
	''' {@code PKIXParameters}, it clones the {@code PKIXRevocationChecker};
	''' thus any subsequent modifications to the {@code PKIXRevocationChecker}
	''' have no effect.
	''' 
	''' <p>Any parameter that is not set (or is set to {@code null}) will be set to
	''' the default value for that parameter.
	''' 
	''' <p><b>Concurrent Access</b>
	''' 
	''' <p>Unless otherwise specified, the methods defined in this class are not
	''' thread-safe. Multiple threads that need to access a single object
	''' concurrently should synchronize amongst themselves and provide the
	''' necessary locking. Multiple threads each manipulating separate objects
	''' need not synchronize.
	''' 
	''' @since 1.8
	''' </summary>
	''' <seealso cref= <a href="http://www.ietf.org/rfc/rfc2560.txt"><i>RFC&nbsp;2560: X.509
	''' Internet Public Key Infrastructure Online Certificate Status Protocol -
	''' OCSP</i></a>, <br><a
	''' href="http://www.ietf.org/rfc/rfc5280.txt"><i>RFC&nbsp;5280: Internet X.509
	''' Public Key Infrastructure Certificate and Certificate Revocation List (CRL)
	''' Profile</i></a> </seealso>
	Public MustInherit Class PKIXRevocationChecker
		Inherits PKIXCertPathChecker

		Private ocspResponder As java.net.URI
		Private ocspResponderCert As X509Certificate
		Private ocspExtensions As IList(Of Extension) = java.util.Collections.emptyList(Of Extension)()
		Private ocspResponses As IDictionary(Of X509Certificate, SByte()) = java.util.Collections.emptyMap()
		Private options As java.util.Set(Of [Option]) = java.util.Collections.emptySet()

		''' <summary>
		''' Default constructor.
		''' </summary>
		Protected Friend Sub New()
		End Sub

		''' <summary>
		''' Sets the URI that identifies the location of the OCSP responder. This
		''' overrides the {@code ocsp.responderURL} security property and any
		''' responder specified in a certificate's Authority Information Access
		''' Extension, as defined in RFC 5280.
		''' </summary>
		''' <param name="uri"> the responder URI </param>
		Public Overridable Property ocspResponder As java.net.URI
			Set(  uri As java.net.URI)
				Me.ocspResponder = uri
			End Set
			Get
				Return ocspResponder
			End Get
		End Property


		''' <summary>
		''' Sets the OCSP responder's certificate. This overrides the
		''' {@code ocsp.responderCertSubjectName},
		''' {@code ocsp.responderCertIssuerName},
		''' and {@code ocsp.responderCertSerialNumber} security properties.
		''' </summary>
		''' <param name="cert"> the responder's certificate </param>
		Public Overridable Property ocspResponderCert As X509Certificate
			Set(  cert As X509Certificate)
				Me.ocspResponderCert = cert
			End Set
			Get
				Return ocspResponderCert
			End Get
		End Property


		' request extensions; single extensions not supported
		''' <summary>
		''' Sets the optional OCSP request extensions.
		''' </summary>
		''' <param name="extensions"> a list of extensions. The list is copied to protect
		'''        against subsequent modification. </param>
		Public Overridable Property ocspExtensions As IList(Of Extension)
			Set(  extensions As IList(Of Extension))
				Me.ocspExtensions = If(extensions Is Nothing, java.util.Collections.emptyList(Of Extension)(), New List(Of Extension)(extensions))
			End Set
			Get
				Return java.util.Collections.unmodifiableList(ocspExtensions)
			End Get
		End Property


		''' <summary>
		''' Sets the OCSP responses. These responses are used to determine
		''' the revocation status of the specified certificates when OCSP is used.
		''' </summary>
		''' <param name="responses"> a map of OCSP responses. Each key is an
		'''        {@code X509Certificate} that maps to the corresponding
		'''        DER-encoded OCSP response for that certificate. A deep copy of
		'''        the map is performed to protect against subsequent modification. </param>
		Public Overridable Property ocspResponses As IDictionary(Of X509Certificate, SByte())
			Set(  responses As IDictionary(Of X509Certificate, SByte()))
				If responses Is Nothing Then
					Me.ocspResponses = java.util.Collections.emptyMap(Of X509Certificate, SByte())()
				Else
					Dim copy As IDictionary(Of X509Certificate, SByte()) = New Dictionary(Of X509Certificate, SByte())(responses.Count)
					For Each e As KeyValuePair(Of X509Certificate, SByte()) In responses
						copy(e.Key) = e.Value.clone()
					Next e
					Me.ocspResponses = copy
				End If
			End Set
			Get
				Dim copy As IDictionary(Of X509Certificate, SByte()) = New Dictionary(Of X509Certificate, SByte())(ocspResponses.Count)
				For Each e As KeyValuePair(Of X509Certificate, SByte()) In ocspResponses
					copy(e.Key) = e.Value.clone()
				Next e
				Return copy
			End Get
		End Property


		''' <summary>
		''' Sets the revocation options.
		''' </summary>
		''' <param name="options"> a set of revocation options. The set is copied to protect
		'''        against subsequent modification. </param>
		Public Overridable Property options As java.util.Set(Of [Option])
			Set(  options As java.util.Set(Of [Option]))
				Me.options = If(options Is Nothing, java.util.Collections.emptySet(Of [Option])(), New HashSet(Of [Option])(options))
			End Set
			Get
				Return java.util.Collections.unmodifiableSet(options)
			End Get
		End Property


		''' <summary>
		''' Returns a list containing the exceptions that are ignored by the
		''' revocation checker when the <seealso cref="Option#SOFT_FAIL SOFT_FAIL"/> option
		''' is set. The list is cleared each time <seealso cref="#init init"/> is called.
		''' The list is ordered in ascending order according to the certificate
		''' index returned by <seealso cref="CertPathValidatorException#getIndex getIndex"/>
		''' method of each entry.
		''' <p>
		''' An implementation of {@code PKIXRevocationChecker} is responsible for
		''' adding the ignored exceptions to the list.
		''' </summary>
		''' <returns> an unmodifiable list containing the ignored exceptions. The list
		'''         is empty if no exceptions have been ignored. </returns>
		Public MustOverride ReadOnly Property softFailExceptions As IList(Of CertPathValidatorException)

		Public Overrides Function clone() As PKIXRevocationChecker
			Dim copy As PKIXRevocationChecker = CType(MyBase.clone(), PKIXRevocationChecker)
			copy.ocspExtensions = New List(Of )(ocspExtensions)
			copy.ocspResponses = New Dictionary(Of )(ocspResponses)
			' deep-copy the encoded responses, since they are mutable
			For Each entry As KeyValuePair(Of X509Certificate, SByte()) In copy.ocspResponses
				Dim encoded As SByte() = entry.Value
				entry.value = encoded.clone()
			Next entry
			copy.options = New HashSet(Of )(options)
			Return copy
		End Function

		''' <summary>
		''' Various revocation options that can be specified for the revocation
		''' checking mechanism.
		''' </summary>
		Public Enum Option
			''' <summary>
			''' Only check the revocation status of end-entity certificates.
			''' </summary>
			ONLY_END_ENTITY
			''' <summary>
			''' Prefer CRLs to OSCP. The default behavior is to prefer OCSP. Each
			''' PKIX implementation should document further details of their
			''' specific preference rules and fallback policies.
			''' </summary>
			PREFER_CRLS
			''' <summary>
			''' Disable the fallback mechanism.
			''' </summary>
			NO_FALLBACK
			''' <summary>
			''' Allow revocation check to succeed if the revocation status cannot be
			''' determined for one of the following reasons:
			''' <ul>
			'''  <li>The CRL or OCSP response cannot be obtained because of a
			'''      network error.
			'''  <li>The OCSP responder returns one of the following errors
			'''      specified in section 2.3 of RFC 2560: internalError or tryLater.
			''' </ul><br>
			''' Note that these conditions apply to both OCSP and CRLs, and unless
			''' the {@code NO_FALLBACK} option is set, the revocation check is
			''' allowed to succeed only if both mechanisms fail under one of the
			''' conditions as stated above.
			''' Exceptions that cause the network errors are ignored but can be
			''' later retrieved by calling the
			''' <seealso cref="#getSoftFailExceptions getSoftFailExceptions"/> method.
			''' </summary>
			SOFT_FAIL
		End Enum
	End Class

End Namespace