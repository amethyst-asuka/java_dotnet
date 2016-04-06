Imports Microsoft.VisualBasic
Imports System

'
' * Copyright (c) 2001, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' A trust anchor or most-trusted Certification Authority (CA).
	''' <p>
	''' This class represents a "most-trusted CA", which is used as a trust anchor
	''' for validating X.509 certification paths. A most-trusted CA includes the
	''' public key of the CA, the CA's name, and any constraints upon the set of
	''' paths which may be validated using this key. These parameters can be
	''' specified in the form of a trusted {@code X509Certificate} or as
	''' individual parameters.
	''' <p>
	''' <b>Concurrent Access</b>
	''' <p>All {@code TrustAnchor} objects must be immutable and
	''' thread-safe. That is, multiple threads may concurrently invoke the
	''' methods defined in this class on a single {@code TrustAnchor}
	''' object (or more than one) with no ill effects. Requiring
	''' {@code TrustAnchor} objects to be immutable and thread-safe
	''' allows them to be passed around to various pieces of code without
	''' worrying about coordinating access. This stipulation applies to all
	''' public fields and methods of this class and any added or overridden
	''' by subclasses.
	''' </summary>
	''' <seealso cref= PKIXParameters#PKIXParameters(Set) </seealso>
	''' <seealso cref= PKIXBuilderParameters#PKIXBuilderParameters(Set, CertSelector)
	''' 
	''' @since       1.4
	''' @author      Sean Mullan </seealso>
	Public Class TrustAnchor

		Private ReadOnly pubKey As java.security.PublicKey
		Private ReadOnly caName As String
		Private ReadOnly caPrincipal As javax.security.auth.x500.X500Principal
		Private ReadOnly trustedCert As X509Certificate
		Private ncBytes As SByte()
		Private nc As sun.security.x509.NameConstraintsExtension

		''' <summary>
		''' Creates an instance of {@code TrustAnchor} with the specified
		''' {@code X509Certificate} and optional name constraints, which
		''' are intended to be used as additional constraints when validating
		''' an X.509 certification path.
		''' <p>
		''' The name constraints are specified as a byte array. This byte array
		''' should contain the DER encoded form of the name constraints, as they
		''' would appear in the NameConstraints structure defined in
		''' <a href="http://www.ietf.org/rfc/rfc3280">RFC 3280</a>
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
		''' Note that the name constraints byte array supplied is cloned to protect
		''' against subsequent modifications.
		''' </summary>
		''' <param name="trustedCert"> a trusted {@code X509Certificate} </param>
		''' <param name="nameConstraints"> a byte array containing the ASN.1 DER encoding of
		''' a NameConstraints extension to be used for checking name constraints.
		''' Only the value of the extension is included, not the OID or criticality
		''' flag. Specify {@code null} to omit the parameter. </param>
		''' <exception cref="IllegalArgumentException"> if the name constraints cannot be
		''' decoded </exception>
		''' <exception cref="NullPointerException"> if the specified
		''' {@code X509Certificate} is {@code null} </exception>
		Public Sub New(  trustedCert As X509Certificate,   nameConstraints As SByte())
			If trustedCert Is Nothing Then Throw New NullPointerException("the trustedCert parameter must " & "be non-null")
			Me.trustedCert = trustedCert
			Me.pubKey = Nothing
			Me.caName = Nothing
			Me.caPrincipal = Nothing
			nameConstraints = nameConstraints
		End Sub

		''' <summary>
		''' Creates an instance of {@code TrustAnchor} where the
		''' most-trusted CA is specified as an X500Principal and public key.
		''' Name constraints are an optional parameter, and are intended to be used
		''' as additional constraints when validating an X.509 certification path.
		''' <p>
		''' The name constraints are specified as a byte array. This byte array
		''' contains the DER encoded form of the name constraints, as they
		''' would appear in the NameConstraints structure defined in RFC 3280
		''' and X.509. The ASN.1 notation for this structure is supplied in the
		''' documentation for
		''' {@link #TrustAnchor(X509Certificate, byte[])
		''' TrustAnchor(X509Certificate trustedCert, byte[] nameConstraints) }.
		''' <p>
		''' Note that the name constraints byte array supplied here is cloned to
		''' protect against subsequent modifications.
		''' </summary>
		''' <param name="caPrincipal"> the name of the most-trusted CA as X500Principal </param>
		''' <param name="pubKey"> the public key of the most-trusted CA </param>
		''' <param name="nameConstraints"> a byte array containing the ASN.1 DER encoding of
		''' a NameConstraints extension to be used for checking name constraints.
		''' Only the value of the extension is included, not the OID or criticality
		''' flag. Specify {@code null} to omit the parameter. </param>
		''' <exception cref="NullPointerException"> if the specified {@code caPrincipal} or
		''' {@code pubKey} parameter is {@code null}
		''' @since 1.5 </exception>
		Public Sub New(  caPrincipal As javax.security.auth.x500.X500Principal,   pubKey As java.security.PublicKey,   nameConstraints As SByte())
			If (caPrincipal Is Nothing) OrElse (pubKey Is Nothing) Then Throw New NullPointerException
			Me.trustedCert = Nothing
			Me.caPrincipal = caPrincipal
			Me.caName = caPrincipal.name
			Me.pubKey = pubKey
			nameConstraints = nameConstraints
		End Sub

		''' <summary>
		''' Creates an instance of {@code TrustAnchor} where the
		''' most-trusted CA is specified as a distinguished name and public key.
		''' Name constraints are an optional parameter, and are intended to be used
		''' as additional constraints when validating an X.509 certification path.
		''' <p>
		''' The name constraints are specified as a byte array. This byte array
		''' contains the DER encoded form of the name constraints, as they
		''' would appear in the NameConstraints structure defined in RFC 3280
		''' and X.509. The ASN.1 notation for this structure is supplied in the
		''' documentation for
		''' {@link #TrustAnchor(X509Certificate, byte[])
		''' TrustAnchor(X509Certificate trustedCert, byte[] nameConstraints) }.
		''' <p>
		''' Note that the name constraints byte array supplied here is cloned to
		''' protect against subsequent modifications.
		''' </summary>
		''' <param name="caName"> the X.500 distinguished name of the most-trusted CA in
		''' <a href="http://www.ietf.org/rfc/rfc2253.txt">RFC 2253</a>
		''' {@code String} format </param>
		''' <param name="pubKey"> the public key of the most-trusted CA </param>
		''' <param name="nameConstraints"> a byte array containing the ASN.1 DER encoding of
		''' a NameConstraints extension to be used for checking name constraints.
		''' Only the value of the extension is included, not the OID or criticality
		''' flag. Specify {@code null} to omit the parameter. </param>
		''' <exception cref="IllegalArgumentException"> if the specified
		''' {@code caName} parameter is empty {@code (caName.length() == 0)}
		''' or incorrectly formatted or the name constraints cannot be decoded </exception>
		''' <exception cref="NullPointerException"> if the specified {@code caName} or
		''' {@code pubKey} parameter is {@code null} </exception>
		Public Sub New(  caName As String,   pubKey As java.security.PublicKey,   nameConstraints As SByte())
			If pubKey Is Nothing Then Throw New NullPointerException("the pubKey parameter must be " & "non-null")
			If caName Is Nothing Then Throw New NullPointerException("the caName parameter must be " & "non-null")
			If caName.length() = 0 Then Throw New IllegalArgumentException("the caName " & "parameter must be a non-empty String")
			' check if caName is formatted correctly
			Me.caPrincipal = New javax.security.auth.x500.X500Principal(caName)
			Me.pubKey = pubKey
			Me.caName = caName
			Me.trustedCert = Nothing
			nameConstraints = nameConstraints
		End Sub

		''' <summary>
		''' Returns the most-trusted CA certificate.
		''' </summary>
		''' <returns> a trusted {@code X509Certificate} or {@code null}
		''' if the trust anchor was not specified as a trusted certificate </returns>
		Public Property trustedCert As X509Certificate
			Get
				Return Me.trustedCert
			End Get
		End Property

		''' <summary>
		''' Returns the name of the most-trusted CA as an X500Principal.
		''' </summary>
		''' <returns> the X.500 distinguished name of the most-trusted CA, or
		''' {@code null} if the trust anchor was not specified as a trusted
		''' public key and name or X500Principal pair
		''' @since 1.5 </returns>
		Public Property cA As javax.security.auth.x500.X500Principal
			Get
				Return Me.caPrincipal
			End Get
		End Property

		''' <summary>
		''' Returns the name of the most-trusted CA in RFC 2253 {@code String}
		''' format.
		''' </summary>
		''' <returns> the X.500 distinguished name of the most-trusted CA, or
		''' {@code null} if the trust anchor was not specified as a trusted
		''' public key and name or X500Principal pair </returns>
		Public Property cAName As String
			Get
				Return Me.caName
			End Get
		End Property

		''' <summary>
		''' Returns the public key of the most-trusted CA.
		''' </summary>
		''' <returns> the public key of the most-trusted CA, or {@code null}
		''' if the trust anchor was not specified as a trusted public key and name
		''' or X500Principal pair </returns>
		Public Property cAPublicKey As java.security.PublicKey
			Get
				Return Me.pubKey
			End Get
		End Property

		''' <summary>
		''' Decode the name constraints and clone them if not null.
		''' </summary>
		Private Property nameConstraints As SByte()
			Set(  bytes As SByte())
				If bytes Is Nothing Then
					ncBytes = Nothing
					nc = Nothing
				Else
					ncBytes = bytes.clone()
					' validate DER encoding
					Try
						nc = New sun.security.x509.NameConstraintsExtension(Boolean.FALSE, bytes)
					Catch ioe As java.io.IOException
						Dim iae As New IllegalArgumentException(ioe.Message)
						iae.initCause(ioe)
						Throw iae
					End Try
				End If
			End Set
			Get
				Return If(ncBytes Is Nothing, Nothing, ncBytes.clone())
			End Get
		End Property


		''' <summary>
		''' Returns a formatted string describing the {@code TrustAnchor}.
		''' </summary>
		''' <returns> a formatted string describing the {@code TrustAnchor} </returns>
		Public Overrides Function ToString() As String
			Dim sb As New StringBuffer
			sb.append("[" & vbLf)
			If pubKey IsNot Nothing Then
				sb.append("  Trusted CA Public Key: " & pubKey.ToString() & vbLf)
				sb.append("  Trusted CA Issuer Name: " & Convert.ToString(caName) & vbLf)
			Else
				sb.append("  Trusted CA cert: " & trustedCert.ToString() & vbLf)
			End If
			If nc IsNot Nothing Then sb.append("  Name Constraints: " & nc.ToString() & vbLf)
			Return sb.ToString()
		End Function
	End Class

End Namespace