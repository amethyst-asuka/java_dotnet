Imports Microsoft.VisualBasic
Imports System.Collections.Generic

'
' * Copyright (c) 2000, 2015, Oracle and/or its affiliates. All rights reserved.
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
	''' A {@code CRLSelector} that selects {@code X509CRLs} that
	''' match all specified criteria. This class is particularly useful when
	''' selecting CRLs from a {@code CertStore} to check revocation status
	''' of a particular certificate.
	''' <p>
	''' When first constructed, an {@code X509CRLSelector} has no criteria
	''' enabled and each of the {@code get} methods return a default
	''' value ({@code null}). Therefore, the <seealso cref="#match match"/> method
	''' would return {@code true} for any {@code X509CRL}. Typically,
	''' several criteria are enabled (by calling <seealso cref="#setIssuers setIssuers"/>
	''' or <seealso cref="#setDateAndTime setDateAndTime"/>, for instance) and then the
	''' {@code X509CRLSelector} is passed to
	''' <seealso cref="CertStore#getCRLs CertStore.getCRLs"/> or some similar
	''' method.
	''' <p>
	''' Please refer to <a href="http://www.ietf.org/rfc/rfc3280.txt">RFC 3280:
	''' Internet X.509 Public Key Infrastructure Certificate and CRL Profile</a>
	''' for definitions of the X.509 CRL fields and extensions mentioned below.
	''' <p>
	''' <b>Concurrent Access</b>
	''' <p>
	''' Unless otherwise specified, the methods defined in this class are not
	''' thread-safe. Multiple threads that need to access a single
	''' object concurrently should synchronize amongst themselves and
	''' provide the necessary locking. Multiple threads each manipulating
	''' separate objects need not synchronize.
	''' </summary>
	''' <seealso cref= CRLSelector </seealso>
	''' <seealso cref= X509CRL
	''' 
	''' @since       1.4
	''' @author      Steve Hanna </seealso>
	Public Class X509CRLSelector
		Implements CRLSelector

		Shared Sub New()
			CertPathHelperImpl.initialize()
		End Sub

		Private Shared ReadOnly debug As sun.security.util.Debug = sun.security.util.Debug.getInstance("certpath")
		Private issuerNames As HashSet(Of Object)
		Private issuerX500Principals As HashSet(Of javax.security.auth.x500.X500Principal)
		Private minCRL As System.Numerics.BigInteger
		Private maxCRL As System.Numerics.BigInteger
		Private dateAndTime As Date
		Private certChecking As X509Certificate
		Private skew As Long = 0

		''' <summary>
		''' Creates an {@code X509CRLSelector}. Initially, no criteria are set
		''' so any {@code X509CRL} will match.
		''' </summary>
		Public Sub New()
		End Sub

		''' <summary>
		''' Sets the issuerNames criterion. The issuer distinguished name in the
		''' {@code X509CRL} must match at least one of the specified
		''' distinguished names. If {@code null}, any issuer distinguished name
		''' will do.
		''' <p>
		''' This method allows the caller to specify, with a single method call,
		''' the complete set of issuer names which {@code X509CRLs} may contain.
		''' The specified value replaces the previous value for the issuerNames
		''' criterion.
		''' <p>
		''' The {@code names} parameter (if not {@code null}) is a
		''' {@code Collection} of {@code X500Principal}s.
		''' <p>
		''' Note that the {@code names} parameter can contain duplicate
		''' distinguished names, but they may be removed from the
		''' {@code Collection} of names returned by the
		''' <seealso cref="#getIssuers getIssuers"/> method.
		''' <p>
		''' Note that a copy is performed on the {@code Collection} to
		''' protect against subsequent modifications.
		''' </summary>
		''' <param name="issuers"> a {@code Collection} of X500Principals
		'''   (or {@code null}) </param>
		''' <seealso cref= #getIssuers
		''' @since 1.5 </seealso>
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
        Public Overridable Sub setIssuers(  issuers As Collection(Of javax.security.auth.x500.X500Principal)) 'JavaToDotNetTempPropertySetissuers
		Public Overridable Property issuers As Collection(Of javax.security.auth.x500.X500Principal)
			Set(  issuers As Collection(Of javax.security.auth.x500.X500Principal))
				If (issuers Is Nothing) OrElse issuers.empty Then
					issuerNames = Nothing
					issuerX500Principals = Nothing
				Else
					' clone
					issuerX500Principals = New HashSet(Of javax.security.auth.x500.X500Principal)(issuers)
					issuerNames = New HashSet(Of Object)
					For Each p As javax.security.auth.x500.X500Principal In issuerX500Principals
						issuerNames.add(p.encoded)
					Next p
				End If
			End Set
			Get
		End Property

		''' <summary>
		''' <strong>Note:</strong> use <seealso cref="#setIssuers(Collection)"/> instead
		''' or only specify the byte array form of distinguished names when using
		''' this method. See <seealso cref="#addIssuerName(String)"/> for more information.
		''' <p>
		''' Sets the issuerNames criterion. The issuer distinguished name in the
		''' {@code X509CRL} must match at least one of the specified
		''' distinguished names. If {@code null}, any issuer distinguished name
		''' will do.
		''' <p>
		''' This method allows the caller to specify, with a single method call,
		''' the complete set of issuer names which {@code X509CRLs} may contain.
		''' The specified value replaces the previous value for the issuerNames
		''' criterion.
		''' <p>
		''' The {@code names} parameter (if not {@code null}) is a
		''' {@code Collection} of names. Each name is a {@code String}
		''' or a byte array representing a distinguished name (in
		''' <a href="http://www.ietf.org/rfc/rfc2253.txt">RFC 2253</a> or
		''' ASN.1 DER encoded form, respectively). If {@code null} is supplied
		''' as the value for this argument, no issuerNames check will be performed.
		''' <p>
		''' Note that the {@code names} parameter can contain duplicate
		''' distinguished names, but they may be removed from the
		''' {@code Collection} of names returned by the
		''' <seealso cref="#getIssuerNames getIssuerNames"/> method.
		''' <p>
		''' If a name is specified as a byte array, it should contain a single DER
		''' encoded distinguished name, as defined in X.501. The ASN.1 notation for
		''' this structure is as follows.
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
		''' Note that a deep copy is performed on the {@code Collection} to
		''' protect against subsequent modifications.
		''' </summary>
		''' <param name="names"> a {@code Collection} of names (or {@code null}) </param>
		''' <exception cref="IOException"> if a parsing error occurs </exception>
		''' <seealso cref= #getIssuerNames </seealso>
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
        Public Overridable Sub setIssuerNames(Of T1)(  names As Collection(Of T1)) 'JavaToDotNetTempPropertySetissuerNames
		Public Overridable Property issuerNames(Of T1) As Collection(Of T1)
			Set(  names As Collection(Of T1))
				If names Is Nothing OrElse names.size() = 0 Then
					issuerNames = Nothing
					issuerX500Principals = Nothing
				Else
					Dim tempNames As HashSet(Of Object) = cloneAndCheckIssuerNames(names)
					' Ensure that we either set both of these or neither
					issuerX500Principals = parseIssuerNames(tempNames)
					issuerNames = tempNames
				End If
			End Set
			Get
		End Property

		''' <summary>
		''' Adds a name to the issuerNames criterion. The issuer distinguished
		''' name in the {@code X509CRL} must match at least one of the specified
		''' distinguished names.
		''' <p>
		''' This method allows the caller to add a name to the set of issuer names
		''' which {@code X509CRLs} may contain. The specified name is added to
		''' any previous value for the issuerNames criterion.
		''' If the specified name is a duplicate, it may be ignored.
		''' </summary>
		''' <param name="issuer"> the issuer as X500Principal
		''' @since 1.5 </param>
		Public Overridable Sub addIssuer(  issuer As javax.security.auth.x500.X500Principal)
			addIssuerNameInternal(issuer.encoded, issuer)
		End Sub

		''' <summary>
		''' <strong>Denigrated</strong>, use
		''' <seealso cref="#addIssuer(X500Principal)"/> or
		''' <seealso cref="#addIssuerName(byte[])"/> instead. This method should not be
		''' relied on as it can fail to match some CRLs because of a loss of
		''' encoding information in the RFC 2253 String form of some distinguished
		''' names.
		''' <p>
		''' Adds a name to the issuerNames criterion. The issuer distinguished
		''' name in the {@code X509CRL} must match at least one of the specified
		''' distinguished names.
		''' <p>
		''' This method allows the caller to add a name to the set of issuer names
		''' which {@code X509CRLs} may contain. The specified name is added to
		''' any previous value for the issuerNames criterion.
		''' If the specified name is a duplicate, it may be ignored.
		''' </summary>
		''' <param name="name"> the name in RFC 2253 form </param>
		''' <exception cref="IOException"> if a parsing error occurs </exception>
		Public Overridable Sub addIssuerName(  name As String)
			addIssuerNameInternal(name, (New sun.security.x509.X500Name(name)).asX500Principal())
		End Sub

		''' <summary>
		''' Adds a name to the issuerNames criterion. The issuer distinguished
		''' name in the {@code X509CRL} must match at least one of the specified
		''' distinguished names.
		''' <p>
		''' This method allows the caller to add a name to the set of issuer names
		''' which {@code X509CRLs} may contain. The specified name is added to
		''' any previous value for the issuerNames criterion. If the specified name
		''' is a duplicate, it may be ignored.
		''' If a name is specified as a byte array, it should contain a single DER
		''' encoded distinguished name, as defined in X.501. The ASN.1 notation for
		''' this structure is as follows.
		''' <p>
		''' The name is provided as a byte array. This byte array should contain
		''' a single DER encoded distinguished name, as defined in X.501. The ASN.1
		''' notation for this structure appears in the documentation for
		''' <seealso cref="#setIssuerNames setIssuerNames(Collection names)"/>.
		''' <p>
		''' Note that the byte array supplied here is cloned to protect against
		''' subsequent modifications.
		''' </summary>
		''' <param name="name"> a byte array containing the name in ASN.1 DER encoded form </param>
		''' <exception cref="IOException"> if a parsing error occurs </exception>
		Public Overridable Sub addIssuerName(  name As SByte())
			' clone because byte arrays are modifiable
			addIssuerNameInternal(name.clone(), (New sun.security.x509.X500Name(name)).asX500Principal())
		End Sub

		''' <summary>
		''' A private method that adds a name (String or byte array) to the
		''' issuerNames criterion. The issuer distinguished
		''' name in the {@code X509CRL} must match at least one of the specified
		''' distinguished names.
		''' </summary>
		''' <param name="name"> the name in string or byte array form </param>
		''' <param name="principal"> the name in X500Principal form </param>
		''' <exception cref="IOException"> if a parsing error occurs </exception>
		Private Sub addIssuerNameInternal(  name As Object,   principal As javax.security.auth.x500.X500Principal)
			If issuerNames Is Nothing Then issuerNames = New HashSet(Of Object)
			If issuerX500Principals Is Nothing Then issuerX500Principals = New HashSet(Of javax.security.auth.x500.X500Principal)
			issuerNames.add(name)
			issuerX500Principals.add(principal)
		End Sub

		''' <summary>
		''' Clone and check an argument of the form passed to
		''' setIssuerNames. Throw an IOException if the argument is malformed.
		''' </summary>
		''' <param name="names"> a {@code Collection} of names. Each entry is a
		'''              String or a byte array (the name, in string or ASN.1
		'''              DER encoded form, respectively). {@code null} is
		'''              not an acceptable value. </param>
		''' <returns> a deep copy of the specified {@code Collection} </returns>
		''' <exception cref="IOException"> if a parsing error occurs </exception>
		Private Shared Function cloneAndCheckIssuerNames(Of T1)(  names As Collection(Of T1)) As HashSet(Of Object)
			Dim namesCopy As New HashSet(Of Object)
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim i As [Iterator](Of ?) = names.GetEnumerator()
			Do While i.MoveNext()
				Dim nameObject As Object = i.Current
				If Not(TypeOf nameObject Is SByte ()) AndAlso Not(TypeOf nameObject Is String) Then Throw New java.io.IOException("name not byte array or String")
				If TypeOf nameObject Is SByte () Then
					namesCopy.add(CType(nameObject, SByte ()).clone())
				Else
					namesCopy.add(nameObject)
				End If
			Loop
			Return (namesCopy)
		End Function

		''' <summary>
		''' Clone an argument of the form passed to setIssuerNames.
		''' Throw a RuntimeException if the argument is malformed.
		''' <p>
		''' This method wraps cloneAndCheckIssuerNames, changing any IOException
		''' into a RuntimeException. This method should be used when the object being
		''' cloned has already been checked, so there should never be any exceptions.
		''' </summary>
		''' <param name="names"> a {@code Collection} of names. Each entry is a
		'''              String or a byte array (the name, in string or ASN.1
		'''              DER encoded form, respectively). {@code null} is
		'''              not an acceptable value. </param>
		''' <returns> a deep copy of the specified {@code Collection} </returns>
		''' <exception cref="RuntimeException"> if a parsing error occurs </exception>
		Private Shared Function cloneIssuerNames(  names As Collection(Of Object)) As HashSet(Of Object)
			Try
				Return cloneAndCheckIssuerNames(names)
			Catch ioe As java.io.IOException
				Throw New RuntimeException(ioe)
			End Try
		End Function

		''' <summary>
		''' Parse an argument of the form passed to setIssuerNames,
		''' returning a Collection of issuerX500Principals.
		''' Throw an IOException if the argument is malformed.
		''' </summary>
		''' <param name="names"> a {@code Collection} of names. Each entry is a
		'''              String or a byte array (the name, in string or ASN.1
		'''              DER encoded form, respectively). <Code>Null</Code> is
		'''              not an acceptable value. </param>
		''' <returns> a HashSet of issuerX500Principals </returns>
		''' <exception cref="IOException"> if a parsing error occurs </exception>
		Private Shared Function parseIssuerNames(  names As Collection(Of Object)) As HashSet(Of javax.security.auth.x500.X500Principal)
			Dim x500Principals As New HashSet(Of javax.security.auth.x500.X500Principal)
			Dim t As [Iterator](Of Object) = names.GetEnumerator()
			Do While t.MoveNext()
				Dim nameObject As Object = t.Current
				If TypeOf nameObject Is String Then
					x500Principals.add((New sun.security.x509.X500Name(CStr(nameObject))).asX500Principal())
				Else
					Try
						x500Principals.add(New javax.security.auth.x500.X500Principal(CType(nameObject, SByte())))
					Catch e As IllegalArgumentException
						Throw CType((New java.io.IOException("Invalid name")).initCause(e), java.io.IOException)
					End Try
				End If
			Loop
			Return x500Principals
		End Function

		''' <summary>
		''' Sets the minCRLNumber criterion. The {@code X509CRL} must have a
		''' CRL number extension whose value is greater than or equal to the
		''' specified value. If {@code null}, no minCRLNumber check will be
		''' done.
		''' </summary>
		''' <param name="minCRL"> the minimum CRL number accepted (or {@code null}) </param>
		Public Overridable Property minCRLNumber As System.Numerics.BigInteger
			Set(  minCRL As System.Numerics.BigInteger)
				Me.minCRL = minCRL
			End Set
		End Property

		''' <summary>
		''' Sets the maxCRLNumber criterion. The {@code X509CRL} must have a
		''' CRL number extension whose value is less than or equal to the
		''' specified value. If {@code null}, no maxCRLNumber check will be
		''' done.
		''' </summary>
		''' <param name="maxCRL"> the maximum CRL number accepted (or {@code null}) </param>
		Public Overridable Property maxCRLNumber As System.Numerics.BigInteger
			Set(  maxCRL As System.Numerics.BigInteger)
				Me.maxCRL = maxCRL
			End Set
		End Property

		''' <summary>
		''' Sets the dateAndTime criterion. The specified date must be
		''' equal to or later than the value of the thisUpdate component
		''' of the {@code X509CRL} and earlier than the value of the
		''' nextUpdate component. There is no match if the {@code X509CRL}
		''' does not contain a nextUpdate component.
		''' If {@code null}, no dateAndTime check will be done.
		''' <p>
		''' Note that the {@code Date} supplied here is cloned to protect
		''' against subsequent modifications.
		''' </summary>
		''' <param name="dateAndTime"> the {@code Date} to match against
		'''                    (or {@code null}) </param>
		''' <seealso cref= #getDateAndTime </seealso>
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
        Public Overridable Sub setDateAndTime(  dateAndTime As Date) 'JavaToDotNetTempPropertySetdateAndTime
		Public Overridable Property dateAndTime As Date
			Set(  dateAndTime As Date)
				If dateAndTime Is Nothing Then
					Me.dateAndTime = Nothing
				Else
					Me.dateAndTime = New Date(dateAndTime.time)
				End If
				Me.skew = 0
			End Set
			Get
		End Property

		''' <summary>
		''' Sets the dateAndTime criterion and allows for the specified clock skew
		''' (in milliseconds) when checking against the validity period of the CRL.
		''' </summary>
		Friend Overridable Sub setDateAndTime(  dateAndTime As Date,   skew As Long)
			Me.dateAndTime = (If(dateAndTime Is Nothing, Nothing, New Date(dateAndTime.time)))
			Me.skew = skew
		End Sub

		''' <summary>
		''' Sets the certificate being checked. This is not a criterion. Rather,
		''' it is optional information that may help a {@code CertStore}
		''' find CRLs that would be relevant when checking revocation for the
		''' specified certificate. If {@code null} is specified, then no
		''' such optional information is provided.
		''' </summary>
		''' <param name="cert"> the {@code X509Certificate} being checked
		'''             (or {@code null}) </param>
		''' <seealso cref= #getCertificateChecking </seealso>
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
        Public Overridable Sub setCertificateChecking(  cert As X509Certificate) 'JavaToDotNetTempPropertySetcertificateChecking
		Public Overridable Property certificateChecking As X509Certificate
			Set(  cert As X509Certificate)
				certChecking = cert
			End Set
			Get
		End Property

			If issuerX500Principals Is Nothing Then Return Nothing
			Return Collections.unmodifiableCollection(issuerX500Principals)
		End Function

			If issuerNames Is Nothing Then Return Nothing
			Return cloneIssuerNames(issuerNames)
		End Function

		''' <summary>
		''' Returns the minCRLNumber criterion. The {@code X509CRL} must have a
		''' CRL number extension whose value is greater than or equal to the
		''' specified value. If {@code null}, no minCRLNumber check will be done.
		''' </summary>
		''' <returns> the minimum CRL number accepted (or {@code null}) </returns>
		Public Overridable Property minCRL As System.Numerics.BigInteger
			Get
				Return minCRL
			End Get
		End Property

		''' <summary>
		''' Returns the maxCRLNumber criterion. The {@code X509CRL} must have a
		''' CRL number extension whose value is less than or equal to the
		''' specified value. If {@code null}, no maxCRLNumber check will be
		''' done.
		''' </summary>
		''' <returns> the maximum CRL number accepted (or {@code null}) </returns>
		Public Overridable Property maxCRL As System.Numerics.BigInteger
			Get
				Return maxCRL
			End Get
		End Property

			If dateAndTime Is Nothing Then Return Nothing
			Return CDate(dateAndTime.clone())
		End Function

			Return certChecking
		End Function

		''' <summary>
		''' Returns a printable representation of the {@code X509CRLSelector}.
		''' </summary>
		''' <returns> a {@code String} describing the contents of the
		'''         {@code X509CRLSelector}. </returns>
		Public Overrides Function ToString() As String
			Dim sb As New StringBuffer
			sb.append("X509CRLSelector: [" & vbLf)
			If issuerNames IsNot Nothing Then
				sb.append("  IssuerNames:" & vbLf)
				Dim i As [Iterator](Of Object) = issuerNames.GetEnumerator()
				Do While i.hasNext()
					sb.append("    " & i.next() & vbLf)
				Loop
			End If
			If minCRL IsNot Nothing Then sb.append("  minCRLNumber: " & minCRL & vbLf)
			If maxCRL IsNot Nothing Then sb.append("  maxCRLNumber: " & maxCRL & vbLf)
			If dateAndTime IsNot Nothing Then sb.append("  dateAndTime: " & dateAndTime & vbLf)
			If certChecking IsNot Nothing Then sb.append("  Certificate being checked: " & certChecking & vbLf)
			sb.append("]")
			Return sb.ToString()
		End Function

		''' <summary>
		''' Decides whether a {@code CRL} should be selected.
		''' </summary>
		''' <param name="crl"> the {@code CRL} to be checked </param>
		''' <returns> {@code true} if the {@code CRL} should be selected,
		'''         {@code false} otherwise </returns>
		Public Overridable Function match(  crl As CRL) As Boolean Implements CRLSelector.match
			If Not(TypeOf crl Is X509CRL) Then Return False
			Dim xcrl As X509CRL = CType(crl, X509CRL)

			' match on issuer name 
			If issuerNames IsNot Nothing Then
				Dim issuer As javax.security.auth.x500.X500Principal = xcrl.issuerX500Principal
				Dim i As [Iterator](Of javax.security.auth.x500.X500Principal) = issuerX500Principals.GetEnumerator()
				Dim found As Boolean = False
				Do While (Not found) AndAlso i.MoveNext()
					If i.Current.Equals(issuer) Then found = True
				Loop
				If Not found Then
					If debug IsNot Nothing Then debug.println("X509CRLSelector.match: issuer DNs " & "don't match")
					Return False
				End If
			End If

			If (minCRL IsNot Nothing) OrElse (maxCRL IsNot Nothing) Then
				' Get CRL number extension from CRL 
				Dim crlNumExtVal As SByte() = xcrl.getExtensionValue("2.5.29.20")
				If crlNumExtVal Is Nothing Then
					If debug IsNot Nothing Then debug.println("X509CRLSelector.match: no CRLNumber")
				End If
				Dim crlNum As System.Numerics.BigInteger
				Try
					Dim [in] As New sun.security.util.DerInputStream(crlNumExtVal)
					Dim encoded As SByte() = [in].octetString
					Dim crlNumExt As New sun.security.x509.CRLNumberExtension(Boolean.FALSE, encoded)
					crlNum = crlNumExt.get(sun.security.x509.CRLNumberExtension.NUMBER)
				Catch ex As java.io.IOException
					If debug IsNot Nothing Then debug.println("X509CRLSelector.match: exception in " & "decoding CRL number")
					Return False
				End Try

				' match on minCRLNumber 
				If minCRL IsNot Nothing Then
					If crlNum.CompareTo(minCRL) < 0 Then
						If debug IsNot Nothing Then debug.println("X509CRLSelector.match: CRLNumber too small")
						Return False
					End If
				End If

				' match on maxCRLNumber 
				If maxCRL IsNot Nothing Then
					If crlNum.CompareTo(maxCRL) > 0 Then
						If debug IsNot Nothing Then debug.println("X509CRLSelector.match: CRLNumber too large")
						Return False
					End If
				End If
			End If


			' match on dateAndTime 
			If dateAndTime IsNot Nothing Then
				Dim crlThisUpdate As Date = xcrl.thisUpdate
				Dim nextUpdate As Date = xcrl.nextUpdate
				If nextUpdate Is Nothing Then
					If debug IsNot Nothing Then debug.println("X509CRLSelector.match: nextUpdate null")
					Return False
				End If
				Dim nowPlusSkew As Date = dateAndTime
				Dim nowMinusSkew As Date = dateAndTime
				If skew > 0 Then
					nowPlusSkew = New Date(dateAndTime.time + skew)
					nowMinusSkew = New Date(dateAndTime.time - skew)
				End If

				' Check that the test date is within the validity interval:
				'   [ thisUpdate - MAX_CLOCK_SKEW,
				'     nextUpdate + MAX_CLOCK_SKEW ]
				If nowMinusSkew.after(nextUpdate) OrElse nowPlusSkew.before(crlThisUpdate) Then
					If debug IsNot Nothing Then debug.println("X509CRLSelector.match: update out-of-range")
					Return False
				End If
			End If

			Return True
		End Function

		''' <summary>
		''' Returns a copy of this object.
		''' </summary>
		''' <returns> the copy </returns>
		Public Overridable Function clone() As Object Implements CRLSelector.clone
			Try
				Dim copy As X509CRLSelector = CType(MyBase.clone(), X509CRLSelector)
				If issuerNames IsNot Nothing Then
					copy.issuerNames = New HashSet(Of Object)(issuerNames)
					copy.issuerX500Principals = New HashSet(Of javax.security.auth.x500.X500Principal)(issuerX500Principals)
				End If
				Return copy
			Catch e As CloneNotSupportedException
				' Cannot happen 
				Throw New InternalError(e.ToString(), e)
			End Try
		End Function
	End Class

End Namespace