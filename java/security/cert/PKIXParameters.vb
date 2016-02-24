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
	''' Parameters used as input for the PKIX {@code CertPathValidator}
	''' algorithm.
	''' <p>
	''' A PKIX {@code CertPathValidator} uses these parameters to
	''' validate a {@code CertPath} according to the PKIX certification path
	''' validation algorithm.
	''' 
	''' <p>To instantiate a {@code PKIXParameters} object, an
	''' application must specify one or more <i>most-trusted CAs</i> as defined by
	''' the PKIX certification path validation algorithm. The most-trusted CAs
	''' can be specified using one of two constructors. An application
	''' can call <seealso cref="#PKIXParameters(Set) PKIXParameters(Set)"/>,
	''' specifying a {@code Set} of {@code TrustAnchor} objects, each
	''' of which identify a most-trusted CA. Alternatively, an application can call
	''' <seealso cref="#PKIXParameters(KeyStore) PKIXParameters(KeyStore)"/>, specifying a
	''' {@code KeyStore} instance containing trusted certificate entries, each
	''' of which will be considered as a most-trusted CA.
	''' <p>
	''' Once a {@code PKIXParameters} object has been created, other parameters
	''' can be specified (by calling <seealso cref="#setInitialPolicies setInitialPolicies"/>
	''' or <seealso cref="#setDate setDate"/>, for instance) and then the
	''' {@code PKIXParameters} is passed along with the {@code CertPath}
	''' to be validated to {@link CertPathValidator#validate
	''' CertPathValidator.validate}.
	''' <p>
	''' Any parameter that is not set (or is set to {@code null}) will
	''' be set to the default value for that parameter. The default value for the
	''' {@code date} parameter is {@code null}, which indicates
	''' the current time when the path is validated. The default for the
	''' remaining parameters is the least constrained.
	''' <p>
	''' <b>Concurrent Access</b>
	''' <p>
	''' Unless otherwise specified, the methods defined in this class are not
	''' thread-safe. Multiple threads that need to access a single
	''' object concurrently should synchronize amongst themselves and
	''' provide the necessary locking. Multiple threads each manipulating
	''' separate objects need not synchronize.
	''' </summary>
	''' <seealso cref= CertPathValidator
	''' 
	''' @since       1.4
	''' @author      Sean Mullan
	''' @author      Yassir Elley </seealso>
	Public Class PKIXParameters
		Implements CertPathParameters

		Private unmodTrustAnchors As java.util.Set(Of TrustAnchor)
		Private [date] As DateTime?
		Private certPathCheckers As IList(Of PKIXCertPathChecker)
		Private sigProvider As String
		Private revocationEnabled As Boolean = True
		Private unmodInitialPolicies As java.util.Set(Of String)
		Private explicitPolicyRequired As Boolean = False
		Private policyMappingInhibited As Boolean = False
		Private anyPolicyInhibited As Boolean = False
		Private policyQualifiersRejected As Boolean = True
		Private certStores As IList(Of CertStore)
		Private certSelector As CertSelector

		''' <summary>
		''' Creates an instance of {@code PKIXParameters} with the specified
		''' {@code Set} of most-trusted CAs. Each element of the
		''' set is a <seealso cref="TrustAnchor TrustAnchor"/>.
		''' <p>
		''' Note that the {@code Set} is copied to protect against
		''' subsequent modifications.
		''' </summary>
		''' <param name="trustAnchors"> a {@code Set} of {@code TrustAnchor}s </param>
		''' <exception cref="InvalidAlgorithmParameterException"> if the specified
		''' {@code Set} is empty {@code (trustAnchors.isEmpty() == true)} </exception>
		''' <exception cref="NullPointerException"> if the specified {@code Set} is
		''' {@code null} </exception>
		''' <exception cref="ClassCastException"> if any of the elements in the {@code Set}
		''' are not of type {@code java.security.cert.TrustAnchor} </exception>
		Public Sub New(ByVal trustAnchors As java.util.Set(Of TrustAnchor))
			trustAnchors = trustAnchors

			Me.unmodInitialPolicies = java.util.Collections.emptySet(Of String)()
			Me.certPathCheckers = New List(Of PKIXCertPathChecker)
			Me.certStores = New List(Of CertStore)
		End Sub

		''' <summary>
		''' Creates an instance of {@code PKIXParameters} that
		''' populates the set of most-trusted CAs from the trusted
		''' certificate entries contained in the specified {@code KeyStore}.
		''' Only keystore entries that contain trusted {@code X509Certificates}
		''' are considered; all other certificate types are ignored.
		''' </summary>
		''' <param name="keystore"> a {@code KeyStore} from which the set of
		''' most-trusted CAs will be populated </param>
		''' <exception cref="KeyStoreException"> if the keystore has not been initialized </exception>
		''' <exception cref="InvalidAlgorithmParameterException"> if the keystore does
		''' not contain at least one trusted certificate entry </exception>
		''' <exception cref="NullPointerException"> if the keystore is {@code null} </exception>
		Public Sub New(ByVal keystore As java.security.KeyStore)
			If keystore Is Nothing Then Throw New NullPointerException("the keystore parameter must be " & "non-null")
			Dim hashSet As java.util.Set(Of TrustAnchor) = New HashSet(Of TrustAnchor)
			Dim aliases As System.Collections.IEnumerator(Of String) = keystore.aliases()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Do While aliases.hasMoreElements()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				Dim [alias] As String = aliases.nextElement()
				If keystore.isCertificateEntry([alias]) Then
					Dim cert As Certificate = keystore.getCertificate([alias])
					If TypeOf cert Is X509Certificate Then hashSet.add(New TrustAnchor(CType(cert, X509Certificate), Nothing))
				End If
			Loop
			trustAnchors = hashSet
			Me.unmodInitialPolicies = java.util.Collections.emptySet(Of String)()
			Me.certPathCheckers = New List(Of PKIXCertPathChecker)
			Me.certStores = New List(Of CertStore)
		End Sub

		''' <summary>
		''' Returns an immutable {@code Set} of the most-trusted
		''' CAs.
		''' </summary>
		''' <returns> an immutable {@code Set} of {@code TrustAnchor}s
		''' (never {@code null})
		''' </returns>
		''' <seealso cref= #setTrustAnchors </seealso>
		Public Overridable Property trustAnchors As java.util.Set(Of TrustAnchor)
			Get
				Return Me.unmodTrustAnchors
			End Get
			Set(ByVal trustAnchors As java.util.Set(Of TrustAnchor))
				If trustAnchors Is Nothing Then Throw New NullPointerException("the trustAnchors parameters must" & " be non-null")
				If trustAnchors.empty Then Throw New java.security.InvalidAlgorithmParameterException("the trustAnchors " & "parameter must be non-empty")
				Dim i As IEnumerator(Of TrustAnchor) = trustAnchors.GetEnumerator()
				Do While i.MoveNext()
					If Not(TypeOf i.Current Is TrustAnchor) Then Throw New ClassCastException("all elements of set must be " & "of type java.security.cert.TrustAnchor")
				Loop
				Me.unmodTrustAnchors = java.util.Collections.unmodifiableSet(New HashSet(Of TrustAnchor)(trustAnchors))
			End Set
		End Property


		''' <summary>
		''' Returns an immutable {@code Set} of initial
		''' policy identifiers (OID strings), indicating that any one of these
		''' policies would be acceptable to the certificate user for the purposes of
		''' certification path processing. The default return value is an empty
		''' {@code Set}, which is interpreted as meaning that any policy would
		''' be acceptable.
		''' </summary>
		''' <returns> an immutable {@code Set} of initial policy OIDs in
		''' {@code String} format, or an empty {@code Set} (implying any
		''' policy is acceptable). Never returns {@code null}.
		''' </returns>
		''' <seealso cref= #setInitialPolicies </seealso>
		Public Overridable Property initialPolicies As java.util.Set(Of String)
			Get
				Return Me.unmodInitialPolicies
			End Get
			Set(ByVal initialPolicies As java.util.Set(Of String))
				If initialPolicies IsNot Nothing Then
					Dim i As IEnumerator(Of String) = initialPolicies.GetEnumerator()
					Do While i.MoveNext()
						If Not(TypeOf i.Current Is String) Then Throw New ClassCastException("all elements of set must be " & "of type java.lang.String")
					Loop
					Me.unmodInitialPolicies = java.util.Collections.unmodifiableSet(New HashSet(Of String)(initialPolicies))
				Else
					Me.unmodInitialPolicies = java.util.Collections.emptySet(Of String)()
				End If
			End Set
		End Property


		''' <summary>
		''' Sets the list of {@code CertStore}s to be used in finding
		''' certificates and CRLs. May be {@code null}, in which case
		''' no {@code CertStore}s will be used. The first
		''' {@code CertStore}s in the list may be preferred to those that
		''' appear later.
		''' <p>
		''' Note that the {@code List} is copied to protect against
		''' subsequent modifications.
		''' </summary>
		''' <param name="stores"> a {@code List} of {@code CertStore}s (or
		''' {@code null}) </param>
		''' <exception cref="ClassCastException"> if any of the elements in the list are
		''' not of type {@code java.security.cert.CertStore}
		''' </exception>
		''' <seealso cref= #getCertStores </seealso>
		Public Overridable Property certStores As IList(Of CertStore)
			Set(ByVal stores As IList(Of CertStore))
				If stores Is Nothing Then
					Me.certStores = New List(Of CertStore)
				Else
					Dim i As IEnumerator(Of CertStore) = stores.GetEnumerator()
					Do While i.MoveNext()
						If Not(TypeOf i.Current Is CertStore) Then Throw New ClassCastException("all elements of list must be " & "of type java.security.cert.CertStore")
					Loop
					Me.certStores = New List(Of CertStore)(stores)
				End If
			End Set
			Get
				Return java.util.Collections.unmodifiableList(New List(Of CertStore)(Me.certStores))
			End Get
		End Property

		''' <summary>
		''' Adds a {@code CertStore} to the end of the list of
		''' {@code CertStore}s used in finding certificates and CRLs.
		''' </summary>
		''' <param name="store"> the {@code CertStore} to add. If {@code null},
		''' the store is ignored (not added to list). </param>
		Public Overridable Sub addCertStore(ByVal store As CertStore)
			If store IsNot Nothing Then Me.certStores.Add(store)
		End Sub


		''' <summary>
		''' Sets the RevocationEnabled flag. If this flag is true, the default
		''' revocation checking mechanism of the underlying PKIX service provider
		''' will be used. If this flag is false, the default revocation checking
		''' mechanism will be disabled (not used).
		''' <p>
		''' When a {@code PKIXParameters} object is created, this flag is set
		''' to true. This setting reflects the most common strategy for checking
		''' revocation, since each service provider must support revocation
		''' checking to be PKIX compliant. Sophisticated applications should set
		''' this flag to false when it is not practical to use a PKIX service
		''' provider's default revocation checking mechanism or when an alternative
		''' revocation checking mechanism is to be substituted (by also calling the
		''' <seealso cref="#addCertPathChecker addCertPathChecker"/> or {@link
		''' #setCertPathCheckers setCertPathCheckers} methods).
		''' </summary>
		''' <param name="val"> the new value of the RevocationEnabled flag </param>
		Public Overridable Property revocationEnabled As Boolean
			Set(ByVal val As Boolean)
				revocationEnabled = val
			End Set
			Get
				Return revocationEnabled
			End Get
		End Property


		''' <summary>
		''' Sets the ExplicitPolicyRequired flag. If this flag is true, an
		''' acceptable policy needs to be explicitly identified in every certificate.
		''' By default, the ExplicitPolicyRequired flag is false.
		''' </summary>
		''' <param name="val"> {@code true} if explicit policy is to be required,
		''' {@code false} otherwise </param>
		Public Overridable Property explicitPolicyRequired As Boolean
			Set(ByVal val As Boolean)
				explicitPolicyRequired = val
			End Set
			Get
				Return explicitPolicyRequired
			End Get
		End Property


		''' <summary>
		''' Sets the PolicyMappingInhibited flag. If this flag is true, policy
		''' mapping is inhibited. By default, policy mapping is not inhibited (the
		''' flag is false).
		''' </summary>
		''' <param name="val"> {@code true} if policy mapping is to be inhibited,
		''' {@code false} otherwise </param>
		Public Overridable Property policyMappingInhibited As Boolean
			Set(ByVal val As Boolean)
				policyMappingInhibited = val
			End Set
			Get
				Return policyMappingInhibited
			End Get
		End Property


		''' <summary>
		''' Sets state to determine if the any policy OID should be processed
		''' if it is included in a certificate. By default, the any policy OID
		''' is not inhibited (<seealso cref="#isAnyPolicyInhibited isAnyPolicyInhibited()"/>
		''' returns {@code false}).
		''' </summary>
		''' <param name="val"> {@code true} if the any policy OID is to be
		''' inhibited, {@code false} otherwise </param>
		Public Overridable Property anyPolicyInhibited As Boolean
			Set(ByVal val As Boolean)
				anyPolicyInhibited = val
			End Set
			Get
				Return anyPolicyInhibited
			End Get
		End Property


		''' <summary>
		''' Sets the PolicyQualifiersRejected flag. If this flag is true,
		''' certificates that include policy qualifiers in a certificate
		''' policies extension that is marked critical are rejected.
		''' If the flag is false, certificates are not rejected on this basis.
		''' 
		''' <p> When a {@code PKIXParameters} object is created, this flag is
		''' set to true. This setting reflects the most common (and simplest)
		''' strategy for processing policy qualifiers. Applications that want to use
		''' a more sophisticated policy must set this flag to false.
		''' <p>
		''' Note that the PKIX certification path validation algorithm specifies
		''' that any policy qualifier in a certificate policies extension that is
		''' marked critical must be processed and validated. Otherwise the
		''' certification path must be rejected. If the policyQualifiersRejected flag
		''' is set to false, it is up to the application to validate all policy
		''' qualifiers in this manner in order to be PKIX compliant.
		''' </summary>
		''' <param name="qualifiersRejected"> the new value of the PolicyQualifiersRejected
		''' flag </param>
		''' <seealso cref= #getPolicyQualifiersRejected </seealso>
		''' <seealso cref= PolicyQualifierInfo </seealso>
		Public Overridable Property policyQualifiersRejected As Boolean
			Set(ByVal qualifiersRejected As Boolean)
				policyQualifiersRejected = qualifiersRejected
			End Set
			Get
				Return policyQualifiersRejected
			End Get
		End Property


		''' <summary>
		''' Returns the time for which the validity of the certification path
		''' should be determined. If {@code null}, the current time is used.
		''' <p>
		''' Note that the {@code Date} returned is copied to protect against
		''' subsequent modifications.
		''' </summary>
		''' <returns> the {@code Date}, or {@code null} if not set </returns>
		''' <seealso cref= #setDate </seealso>
		Public Overridable Property [date] As DateTime?
			Get
				If [date] Is Nothing Then
					Return Nothing
				Else
					Return CDate(Me.date.Value.clone())
				End If
			End Get
			Set(ByVal [date] As DateTime?)
				If date_Renamed IsNot Nothing Then
					Me.date = CDate(date_Renamed.Value.clone())
				Else
					date_Renamed = Nothing
				End If
			End Set
		End Property


		''' <summary>
		''' Sets a {@code List} of additional certification path checkers. If
		''' the specified {@code List} contains an object that is not a
		''' {@code PKIXCertPathChecker}, it is ignored.
		''' <p>
		''' Each {@code PKIXCertPathChecker} specified implements
		''' additional checks on a certificate. Typically, these are checks to
		''' process and verify private extensions contained in certificates.
		''' Each {@code PKIXCertPathChecker} should be instantiated with any
		''' initialization parameters needed to execute the check.
		''' <p>
		''' This method allows sophisticated applications to extend a PKIX
		''' {@code CertPathValidator} or {@code CertPathBuilder}.
		''' Each of the specified {@code PKIXCertPathChecker}s will be called,
		''' in turn, by a PKIX {@code CertPathValidator} or
		''' {@code CertPathBuilder} for each certificate processed or
		''' validated.
		''' <p>
		''' Regardless of whether these additional {@code PKIXCertPathChecker}s
		''' are set, a PKIX {@code CertPathValidator} or
		''' {@code CertPathBuilder} must perform all of the required PKIX
		''' checks on each certificate. The one exception to this rule is if the
		''' RevocationEnabled flag is set to false (see the {@link
		''' #setRevocationEnabled setRevocationEnabled} method).
		''' <p>
		''' Note that the {@code List} supplied here is copied and each
		''' {@code PKIXCertPathChecker} in the list is cloned to protect
		''' against subsequent modifications.
		''' </summary>
		''' <param name="checkers"> a {@code List} of {@code PKIXCertPathChecker}s.
		''' May be {@code null}, in which case no additional checkers will be
		''' used. </param>
		''' <exception cref="ClassCastException"> if any of the elements in the list
		''' are not of type {@code java.security.cert.PKIXCertPathChecker} </exception>
		''' <seealso cref= #getCertPathCheckers </seealso>
		Public Overridable Property certPathCheckers As IList(Of PKIXCertPathChecker)
			Set(ByVal checkers As IList(Of PKIXCertPathChecker))
				If checkers IsNot Nothing Then
					Dim tmpList As IList(Of PKIXCertPathChecker) = New List(Of PKIXCertPathChecker)
					For Each checker As PKIXCertPathChecker In checkers
						tmpList.Add(CType(checker.clone(), PKIXCertPathChecker))
					Next checker
					Me.certPathCheckers = tmpList
				Else
					Me.certPathCheckers = New List(Of PKIXCertPathChecker)
				End If
			End Set
			Get
				Dim tmpList As IList(Of PKIXCertPathChecker) = New List(Of PKIXCertPathChecker)
				For Each ck As PKIXCertPathChecker In certPathCheckers
					tmpList.Add(CType(ck.clone(), PKIXCertPathChecker))
				Next ck
				Return java.util.Collections.unmodifiableList(tmpList)
			End Get
		End Property


		''' <summary>
		''' Adds a {@code PKIXCertPathChecker} to the list of certification
		''' path checkers. See the <seealso cref="#setCertPathCheckers setCertPathCheckers"/>
		''' method for more details.
		''' <p>
		''' Note that the {@code PKIXCertPathChecker} is cloned to protect
		''' against subsequent modifications.
		''' </summary>
		''' <param name="checker"> a {@code PKIXCertPathChecker} to add to the list of
		''' checks. If {@code null}, the checker is ignored (not added to list). </param>
		Public Overridable Sub addCertPathChecker(ByVal checker As PKIXCertPathChecker)
			If checker IsNot Nothing Then certPathCheckers.Add(CType(checker.clone(), PKIXCertPathChecker))
		End Sub

		''' <summary>
		''' Returns the signature provider's name, or {@code null}
		''' if not set.
		''' </summary>
		''' <returns> the signature provider's name (or {@code null}) </returns>
		''' <seealso cref= #setSigProvider </seealso>
		Public Overridable Property sigProvider As String
			Get
				Return Me.sigProvider
			End Get
			Set(ByVal sigProvider As String)
				Me.sigProvider = sigProvider
			End Set
		End Property


		''' <summary>
		''' Returns the required constraints on the target certificate.
		''' The constraints are returned as an instance of {@code CertSelector}.
		''' If {@code null}, no constraints are defined.
		''' 
		''' <p>Note that the {@code CertSelector} returned is cloned
		''' to protect against subsequent modifications.
		''' </summary>
		''' <returns> a {@code CertSelector} specifying the constraints
		''' on the target certificate (or {@code null}) </returns>
		''' <seealso cref= #setTargetCertConstraints </seealso>
		Public Overridable Property targetCertConstraints As CertSelector
			Get
				If certSelector IsNot Nothing Then
					Return CType(certSelector.clone(), CertSelector)
				Else
					Return Nothing
				End If
			End Get
			Set(ByVal selector As CertSelector)
				If selector IsNot Nothing Then
					certSelector = CType(selector.clone(), CertSelector)
				Else
					certSelector = Nothing
				End If
			End Set
		End Property


		''' <summary>
		''' Makes a copy of this {@code PKIXParameters} object. Changes
		''' to the copy will not affect the original and vice versa.
		''' </summary>
		''' <returns> a copy of this {@code PKIXParameters} object </returns>
		Public Overridable Function clone() As Object Implements CertPathParameters.clone
			Try
				Dim copy As PKIXParameters = CType(MyBase.clone(), PKIXParameters)

				' must clone these because addCertStore, et al. modify them
				If certStores IsNot Nothing Then copy.certStores = New List(Of CertStore)(certStores)
				If certPathCheckers IsNot Nothing Then
					copy.certPathCheckers = New List(Of PKIXCertPathChecker)(certPathCheckers.Count)
					For Each checker As PKIXCertPathChecker In certPathCheckers
						copy.certPathCheckers.Add(CType(checker.clone(), PKIXCertPathChecker))
					Next checker
				End If

				' other class fields are immutable to public, don't bother
				' to clone the read-only fields.
				Return copy
			Catch e As CloneNotSupportedException
				' Cannot happen 
				Throw New InternalError(e.ToString(), e)
			End Try
		End Function

		''' <summary>
		''' Returns a formatted string describing the parameters.
		''' </summary>
		''' <returns> a formatted string describing the parameters. </returns>
		Public Overrides Function ToString() As String
			Dim sb As New StringBuffer
			sb.append("[" & vbLf)

			' start with trusted anchor info 
			If unmodTrustAnchors IsNot Nothing Then sb.append("  Trust Anchors: " & unmodTrustAnchors.ToString() & vbLf)

			' now, append initial state information 
			If unmodInitialPolicies IsNot Nothing Then
				If unmodInitialPolicies.empty Then
					sb.append("  Initial Policy OIDs: any" & vbLf)
				Else
					sb.append("  Initial Policy OIDs: [" & unmodInitialPolicies.ToString() & "]" & vbLf)
				End If
			End If

			' now, append constraints on all certificates in the path 
			sb.append("  Validity Date: " & Convert.ToString([date]) & vbLf)
			sb.append("  Signature Provider: " & Convert.ToString(sigProvider) & vbLf)
			sb.append("  Default Revocation Enabled: " & revocationEnabled & vbLf)
			sb.append("  Explicit Policy Required: " & explicitPolicyRequired & vbLf)
			sb.append("  Policy Mapping Inhibited: " & policyMappingInhibited & vbLf)
			sb.append("  Any Policy Inhibited: " & anyPolicyInhibited & vbLf)
			sb.append("  Policy Qualifiers Rejected: " & policyQualifiersRejected & vbLf)

			' now, append target cert requirements 
			sb.append("  Target Cert Constraints: " & Convert.ToString(certSelector) & vbLf)

			' finally, append miscellaneous parameters 
			If certPathCheckers IsNot Nothing Then sb.append("  Certification Path Checkers: [" & certPathCheckers.ToString() & "]" & vbLf)
			If certStores IsNot Nothing Then sb.append("  CertStores: [" & certStores.ToString() & "]" & vbLf)
			sb.append("]")
			Return sb.ToString()
		End Function
	End Class

End Namespace