Imports System.Collections
Imports javax.xml.crypto.dsig
Imports sun.security.jca

'
' * Copyright (c) 2005, 2011, Oracle and/or its affiliates. All rights reserved.
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
'
' * $Id: KeyInfoFactory.java,v 1.12 2005/05/10 16:35:35 mullan Exp $
' 
Namespace javax.xml.crypto.dsig.keyinfo


	''' <summary>
	''' A factory for creating <seealso cref="KeyInfo"/> objects from scratch or for
	''' unmarshalling a <code>KeyInfo</code> object from a corresponding XML
	''' representation.
	''' 
	''' <p>Each instance of <code>KeyInfoFactory</code> supports a specific
	''' XML mechanism type. To create a <code>KeyInfoFactory</code>, call one of the
	''' static <seealso cref="#getInstance getInstance"/> methods, passing in the XML
	''' mechanism type desired, for example:
	''' 
	''' <blockquote><code>
	'''   KeyInfoFactory factory = KeyInfoFactory.getInstance("DOM");
	''' </code></blockquote>
	''' 
	''' <p>The objects that this factory produces will be based
	''' on DOM and abide by the DOM interoperability requirements as defined in the
	''' <a href="../../../../../../technotes/guides/security/xmldsig/overview.html#DOM Mechanism Requirements">
	''' DOM Mechanism Requirements</a> section of the API overview. See the
	''' <a href="../../../../../../technotes/guides/security/xmldsig/overview.html#Service Provider">
	''' Service Providers</a> section of the API overview for a list of standard
	''' mechanism types.
	''' 
	''' <p><code>KeyInfoFactory</code> implementations are registered and loaded
	''' using the <seealso cref="java.security.Provider"/> mechanism.
	''' For example, a service provider that supports the
	''' DOM mechanism would be specified in the <code>Provider</code> subclass as:
	''' <pre>
	'''     put("KeyInfoFactory.DOM", "org.example.DOMKeyInfoFactory");
	''' </pre>
	''' 
	''' <p>Also, the <code>XMLStructure</code>s that are created by this factory
	''' may contain state specific to the <code>KeyInfo</code> and are not
	''' intended to be reusable.
	''' 
	''' <p>An implementation MUST minimally support the default mechanism type: DOM.
	''' 
	''' <p>Note that a caller must use the same <code>KeyInfoFactory</code>
	''' instance to create the <code>XMLStructure</code>s of a particular
	''' <code>KeyInfo</code> object. The behavior is undefined if
	''' <code>XMLStructure</code>s from different providers or different mechanism
	''' types are used together.
	''' 
	''' <p><b>Concurrent Access</b>
	''' <p>The static methods of this class are guaranteed to be thread-safe.
	''' Multiple threads may concurrently invoke the static methods defined in this
	''' class with no ill effects.
	''' 
	''' <p>However, this is not true for the non-static methods defined by this
	''' class. Unless otherwise documented by a specific provider, threads that
	''' need to access a single <code>KeyInfoFactory</code> instance concurrently
	''' should synchronize amongst themselves and provide the necessary locking.
	''' Multiple threads each manipulating a different <code>KeyInfoFactory</code>
	''' instance need not synchronize.
	''' 
	''' @author Sean Mullan
	''' @author JSR 105 Expert Group
	''' @since 1.6
	''' </summary>
	Public MustInherit Class KeyInfoFactory

		Private mechanismType As String
		Private provider As java.security.Provider

		''' <summary>
		''' Default constructor, for invocation by subclasses.
		''' </summary>
		Protected Friend Sub New()
		End Sub

		''' <summary>
		''' Returns a <code>KeyInfoFactory</code> that supports the
		''' specified XML processing mechanism and representation type (ex: "DOM").
		''' 
		''' <p>This method uses the standard JCA provider lookup mechanism to
		''' locate and instantiate a <code>KeyInfoFactory</code> implementation of
		''' the desired mechanism type. It traverses the list of registered security
		''' <code>Provider</code>s, starting with the most preferred
		''' <code>Provider</code>. A new <code>KeyInfoFactory</code> object
		''' from the first <code>Provider</code> that supports the specified
		''' mechanism is returned.
		''' 
		''' <p> Note that the list of registered providers may be retrieved via
		''' the <seealso cref="Security#getProviders() Security.getProviders()"/> method.
		''' </summary>
		''' <param name="mechanismType"> the type of the XML processing mechanism and
		'''    representation. See the <a
		'''    href="../../../../../../technotes/guides/security/xmldsig/overview.html#Service Provider">
		'''    Service Providers</a> section of the API overview for a list of
		'''    standard mechanism types. </param>
		''' <returns> a new <code>KeyInfoFactory</code> </returns>
		''' <exception cref="NullPointerException"> if <code>mechanismType</code> is
		'''    <code>null</code> </exception>
		''' <exception cref="NoSuchMechanismException"> if no <code>Provider</code> supports a
		'''    <code>KeyInfoFactory</code> implementation for the specified mechanism </exception>
		''' <seealso cref= Provider </seealso>
		Public Shared Function getInstance(ByVal mechanismType As String) As KeyInfoFactory
			If mechanismType Is Nothing Then Throw New NullPointerException("mechanismType cannot be null")
			Dim ___instance As sun.security.jca.GetInstance.Instance
			Try
				___instance = GetInstance.getInstance("KeyInfoFactory", Nothing, mechanismType)
			Catch nsae As java.security.NoSuchAlgorithmException
				Throw New javax.xml.crypto.NoSuchMechanismException(nsae)
			End Try
			Dim factory As KeyInfoFactory = CType(___instance.impl, KeyInfoFactory)
			factory.mechanismType = mechanismType
			factory.provider = ___instance.provider
			Return factory
		End Function

		''' <summary>
		''' Returns a <code>KeyInfoFactory</code> that supports the
		''' requested XML processing mechanism and representation type (ex: "DOM"),
		''' as supplied by the specified provider. Note that the specified
		''' <code>Provider</code> object does not have to be registered in the
		''' provider list.
		''' </summary>
		''' <param name="mechanismType"> the type of the XML processing mechanism and
		'''    representation. See the <a
		'''    href="../../../../../../technotes/guides/security/xmldsig/overview.html#Service Provider">
		'''    Service Providers</a> section of the API overview for a list of
		'''    standard mechanism types. </param>
		''' <param name="provider"> the <code>Provider</code> object </param>
		''' <returns> a new <code>KeyInfoFactory</code> </returns>
		''' <exception cref="NullPointerException"> if <code>mechanismType</code> or
		'''    <code>provider</code> are <code>null</code> </exception>
		''' <exception cref="NoSuchMechanismException"> if a <code>KeyInfoFactory</code>
		'''    implementation for the specified mechanism is not available from the
		'''    specified <code>Provider</code> object </exception>
		''' <seealso cref= Provider </seealso>
		Public Shared Function getInstance(ByVal mechanismType As String, ByVal provider As java.security.Provider) As KeyInfoFactory
			If mechanismType Is Nothing Then
				Throw New NullPointerException("mechanismType cannot be null")
			ElseIf provider Is Nothing Then
				Throw New NullPointerException("provider cannot be null")
			End If

			Dim ___instance As sun.security.jca.GetInstance.Instance
			Try
				___instance = GetInstance.getInstance("KeyInfoFactory", Nothing, mechanismType, provider)
			Catch nsae As java.security.NoSuchAlgorithmException
				Throw New javax.xml.crypto.NoSuchMechanismException(nsae)
			End Try
			Dim factory As KeyInfoFactory = CType(___instance.impl, KeyInfoFactory)
			factory.mechanismType = mechanismType
			factory.provider = ___instance.provider
			Return factory
		End Function

		''' <summary>
		''' Returns a <code>KeyInfoFactory</code> that supports the
		''' requested XML processing mechanism and representation type (ex: "DOM"),
		''' as supplied by the specified provider. The specified provider must be
		''' registered in the security provider list.
		''' 
		''' <p>Note that the list of registered providers may be retrieved via
		''' the <seealso cref="Security#getProviders() Security.getProviders()"/> method.
		''' </summary>
		''' <param name="mechanismType"> the type of the XML processing mechanism and
		'''    representation. See the <a
		'''    href="../../../../../../technotes/guides/security/xmldsig/overview.html#Service Provider">
		'''    Service Providers</a> section of the API overview for a list of
		'''    standard mechanism types. </param>
		''' <param name="provider"> the string name of the provider </param>
		''' <returns> a new <code>KeyInfoFactory</code> </returns>
		''' <exception cref="NoSuchProviderException"> if the specified provider is not
		'''    registered in the security provider list </exception>
		''' <exception cref="NullPointerException"> if <code>mechanismType</code> or
		'''    <code>provider</code> are <code>null</code> </exception>
		''' <exception cref="NoSuchMechanismException"> if a <code>KeyInfoFactory</code>
		'''    implementation for the specified mechanism is not available from the
		'''    specified provider </exception>
		''' <seealso cref= Provider </seealso>
		Public Shared Function getInstance(ByVal mechanismType As String, ByVal provider As String) As KeyInfoFactory
			If mechanismType Is Nothing Then
				Throw New NullPointerException("mechanismType cannot be null")
			ElseIf provider Is Nothing Then
				Throw New NullPointerException("provider cannot be null")
			ElseIf provider.Length = 0 Then
				Throw New java.security.NoSuchProviderException
			End If

			Dim ___instance As sun.security.jca.GetInstance.Instance
			Try
				___instance = GetInstance.getInstance("KeyInfoFactory", Nothing, mechanismType, provider)
			Catch nsae As java.security.NoSuchAlgorithmException
				Throw New javax.xml.crypto.NoSuchMechanismException(nsae)
			End Try
			Dim factory As KeyInfoFactory = CType(___instance.impl, KeyInfoFactory)
			factory.mechanismType = mechanismType
			factory.provider = ___instance.provider
			Return factory
		End Function

		''' <summary>
		''' Returns a <code>KeyInfoFactory</code> that supports the
		''' default XML processing mechanism and representation type ("DOM").
		''' 
		''' <p>This method uses the standard JCA provider lookup mechanism to
		''' locate and instantiate a <code>KeyInfoFactory</code> implementation of
		''' the default mechanism type. It traverses the list of registered security
		''' <code>Provider</code>s, starting with the most preferred
		''' <code>Provider</code>.  A new <code>KeyInfoFactory</code> object
		''' from the first <code>Provider</code> that supports the DOM mechanism is
		''' returned.
		''' 
		''' <p> Note that the list of registered providers may be retrieved via
		''' the <seealso cref="Security#getProviders() Security.getProviders()"/> method.
		''' </summary>
		''' <returns> a new <code>KeyInfoFactory</code> </returns>
		''' <exception cref="NoSuchMechanismException"> if no <code>Provider</code> supports a
		'''    <code>KeyInfoFactory</code> implementation for the DOM mechanism </exception>
		''' <seealso cref= Provider </seealso>
		Public Property Shared instance As KeyInfoFactory
			Get
				Return getInstance("DOM")
			End Get
		End Property

		''' <summary>
		''' Returns the type of the XML processing mechanism and representation
		''' supported by this <code>KeyInfoFactory</code> (ex: "DOM")
		''' </summary>
		''' <returns> the XML processing mechanism type supported by this
		'''    <code>KeyInfoFactory</code> </returns>
		Public Property mechanismType As String
			Get
				Return mechanismType
			End Get
		End Property

		''' <summary>
		''' Returns the provider of this <code>KeyInfoFactory</code>.
		''' </summary>
		''' <returns> the provider of this <code>KeyInfoFactory</code> </returns>
		Public Property provider As java.security.Provider
			Get
				Return provider
			End Get
		End Property

		''' <summary>
		''' Creates a <code>KeyInfo</code> containing the specified list of
		''' key information types.
		''' </summary>
		''' <param name="content"> a list of one or more <seealso cref="XMLStructure"/>s representing
		'''    key information types. The list is defensively copied to protect
		'''    against subsequent modification. </param>
		''' <returns> a <code>KeyInfo</code> </returns>
		''' <exception cref="NullPointerException"> if <code>content</code> is <code>null</code> </exception>
		''' <exception cref="IllegalArgumentException"> if <code>content</code> is empty </exception>
		''' <exception cref="ClassCastException"> if <code>content</code> contains any entries
		'''    that are not of type <seealso cref="XMLStructure"/> </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public MustOverride Function newKeyInfo(ByVal content As IList) As KeyInfo

		''' <summary>
		''' Creates a <code>KeyInfo</code> containing the specified list of key
		''' information types and optional id. The
		''' <code>id</code> parameter represents the value of an XML
		''' <code>ID</code> attribute and is useful for referencing
		''' the <code>KeyInfo</code> from other XML structures.
		''' </summary>
		''' <param name="content"> a list of one or more <seealso cref="XMLStructure"/>s representing
		'''    key information types. The list is defensively copied to protect
		'''    against subsequent modification. </param>
		''' <param name="id"> the value of an XML <code>ID</code> (may be <code>null</code>) </param>
		''' <returns> a <code>KeyInfo</code> </returns>
		''' <exception cref="NullPointerException"> if <code>content</code> is <code>null</code> </exception>
		''' <exception cref="IllegalArgumentException"> if <code>content</code> is empty </exception>
		''' <exception cref="ClassCastException"> if <code>content</code> contains any entries
		'''    that are not of type <seealso cref="XMLStructure"/> </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public MustOverride Function newKeyInfo(ByVal content As IList, ByVal id As String) As KeyInfo

		''' <summary>
		''' Creates a <code>KeyName</code> from the specified name.
		''' </summary>
		''' <param name="name"> the name that identifies the key </param>
		''' <returns> a <code>KeyName</code> </returns>
		''' <exception cref="NullPointerException"> if <code>name</code> is <code>null</code> </exception>
		Public MustOverride Function newKeyName(ByVal name As String) As KeyName

		''' <summary>
		''' Creates a <code>KeyValue</code> from the specified public key.
		''' </summary>
		''' <param name="key"> the public key </param>
		''' <returns> a <code>KeyValue</code> </returns>
		''' <exception cref="KeyException"> if the <code>key</code>'s algorithm is not
		'''    recognized or supported by this <code>KeyInfoFactory</code> </exception>
		''' <exception cref="NullPointerException"> if <code>key</code> is <code>null</code> </exception>
		Public MustOverride Function newKeyValue(ByVal key As java.security.PublicKey) As KeyValue

		''' <summary>
		''' Creates a <code>PGPData</code> from the specified PGP public key
		''' identifier.
		''' </summary>
		''' <param name="keyId"> a PGP public key identifier as defined in <a href=
		'''    "http://www.ietf.org/rfc/rfc2440.txt">RFC 2440</a>, section 11.2.
		'''    The array is cloned to protect against subsequent modification. </param>
		''' <returns> a <code>PGPData</code> </returns>
		''' <exception cref="NullPointerException"> if <code>keyId</code> is <code>null</code> </exception>
		''' <exception cref="IllegalArgumentException"> if the key id is not in the correct
		'''    format </exception>
		Public MustOverride Function newPGPData(ByVal keyId As SByte()) As PGPData

		''' <summary>
		''' Creates a <code>PGPData</code> from the specified PGP public key
		''' identifier, and optional key material packet and list of external
		''' elements.
		''' </summary>
		''' <param name="keyId"> a PGP public key identifier as defined in <a href=
		'''    "http://www.ietf.org/rfc/rfc2440.txt">RFC 2440</a>, section 11.2.
		'''    The array is cloned to protect against subsequent modification. </param>
		''' <param name="keyPacket"> a PGP key material packet as defined in <a href=
		'''    "http://www.ietf.org/rfc/rfc2440.txt">RFC 2440</a>, section 5.5.
		'''    The array is cloned to protect against subsequent modification. May
		'''    be <code>null</code>. </param>
		''' <param name="other"> a list of <seealso cref="XMLStructure"/>s representing elements from
		'''    an external namespace. The list is defensively copied to protect
		'''    against subsequent modification. May be <code>null</code> or empty. </param>
		''' <returns> a <code>PGPData</code> </returns>
		''' <exception cref="NullPointerException"> if <code>keyId</code> is <code>null</code> </exception>
		''' <exception cref="IllegalArgumentException"> if the <code>keyId</code> or
		'''    <code>keyPacket</code> is not in the correct format. For
		'''    <code>keyPacket</code>, the format of the packet header is
		'''    checked and the tag is verified that it is of type key material. The
		'''    contents and format of the packet body are not checked. </exception>
		''' <exception cref="ClassCastException"> if <code>other</code> contains any
		'''    entries that are not of type <seealso cref="XMLStructure"/> </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public MustOverride Function newPGPData(ByVal keyId As SByte(), ByVal keyPacket As SByte(), ByVal other As IList) As PGPData

		''' <summary>
		''' Creates a <code>PGPData</code> from the specified PGP key material
		''' packet and optional list of external elements.
		''' </summary>
		''' <param name="keyPacket"> a PGP key material packet as defined in <a href=
		'''    "http://www.ietf.org/rfc/rfc2440.txt">RFC 2440</a>, section 5.5.
		'''    The array is cloned to protect against subsequent modification. </param>
		''' <param name="other"> a list of <seealso cref="XMLStructure"/>s representing elements from
		'''    an external namespace. The list is defensively copied to protect
		'''    against subsequent modification. May be <code>null</code> or empty. </param>
		''' <returns> a <code>PGPData</code> </returns>
		''' <exception cref="NullPointerException"> if <code>keyPacket</code> is
		'''    <code>null</code> </exception>
		''' <exception cref="IllegalArgumentException"> if <code>keyPacket</code> is not in the
		'''    correct format. For <code>keyPacket</code>, the format of the packet
		'''    header is checked and the tag is verified that it is of type key
		'''    material. The contents and format of the packet body are not checked. </exception>
		''' <exception cref="ClassCastException"> if <code>other</code> contains any
		'''    entries that are not of type <seealso cref="XMLStructure"/> </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public MustOverride Function newPGPData(ByVal keyPacket As SByte(), ByVal other As IList) As PGPData

		''' <summary>
		''' Creates a <code>RetrievalMethod</code> from the specified URI.
		''' </summary>
		''' <param name="uri"> the URI that identifies the <code>KeyInfo</code> information
		'''    to be retrieved </param>
		''' <returns> a <code>RetrievalMethod</code> </returns>
		''' <exception cref="NullPointerException"> if <code>uri</code> is <code>null</code> </exception>
		''' <exception cref="IllegalArgumentException"> if <code>uri</code> is not RFC 2396
		'''    compliant </exception>
		Public MustOverride Function newRetrievalMethod(ByVal uri As String) As RetrievalMethod

		''' <summary>
		''' Creates a <code>RetrievalMethod</code> from the specified parameters.
		''' </summary>
		''' <param name="uri"> the URI that identifies the <code>KeyInfo</code> information
		'''    to be retrieved </param>
		''' <param name="type"> a URI that identifies the type of <code>KeyInfo</code>
		'''    information to be retrieved (may be <code>null</code>) </param>
		''' <param name="transforms"> a list of <seealso cref="Transform"/>s. The list is defensively
		'''    copied to protect against subsequent modification. May be
		'''    <code>null</code> or empty. </param>
		''' <returns> a <code>RetrievalMethod</code> </returns>
		''' <exception cref="NullPointerException"> if <code>uri</code> is <code>null</code> </exception>
		''' <exception cref="IllegalArgumentException"> if <code>uri</code> is not RFC 2396
		'''    compliant </exception>
		''' <exception cref="ClassCastException"> if <code>transforms</code> contains any
		'''    entries that are not of type <seealso cref="Transform"/> </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public MustOverride Function newRetrievalMethod(ByVal uri As String, ByVal type As String, ByVal transforms As IList) As RetrievalMethod

		''' <summary>
		''' Creates a <code>X509Data</code> containing the specified list of
		''' X.509 content.
		''' </summary>
		''' <param name="content"> a list of one or more X.509 content types. Valid types are
		'''    <seealso cref="String"/> (subject names), <code>byte[]</code> (subject key ids),
		'''    <seealso cref="java.security.cert.X509Certificate"/>, <seealso cref="X509CRL"/>,
		'''    or <seealso cref="XMLStructure"/> (<seealso cref="X509IssuerSerial"/>
		'''    objects or elements from an external namespace). Subject names are
		'''    distinguished names in RFC 2253 String format. Implementations MUST
		'''    support the attribute type keywords defined in RFC 2253 (CN, L, ST,
		'''    O, OU, C, STREET, DC and UID). Implementations MAY support additional
		'''    keywords. The list is defensively copied to protect against
		'''    subsequent modification. </param>
		''' <returns> a <code>X509Data</code> </returns>
		''' <exception cref="NullPointerException"> if <code>content</code> is <code>null</code> </exception>
		''' <exception cref="IllegalArgumentException"> if <code>content</code> is empty, or
		'''    if a subject name is not RFC 2253 compliant or one of the attribute
		'''    type keywords is not recognized. </exception>
		''' <exception cref="ClassCastException"> if <code>content</code> contains any entries
		'''    that are not of one of the valid types mentioned above </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public MustOverride Function newX509Data(ByVal content As IList) As X509Data

		''' <summary>
		''' Creates an <code>X509IssuerSerial</code> from the specified X.500 issuer
		''' distinguished name and serial number.
		''' </summary>
		''' <param name="issuerName"> the issuer's distinguished name in RFC 2253 String
		'''    format. Implementations MUST support the attribute type keywords
		'''    defined in RFC 2253 (CN, L, ST, O, OU, C, STREET, DC and UID).
		'''    Implementations MAY support additional keywords. </param>
		''' <param name="serialNumber"> the serial number </param>
		''' <returns> an <code>X509IssuerSerial</code> </returns>
		''' <exception cref="NullPointerException"> if <code>issuerName</code> or
		'''    <code>serialNumber</code> are <code>null</code> </exception>
		''' <exception cref="IllegalArgumentException"> if the issuer name is not RFC 2253
		'''    compliant or one of the attribute type keywords is not recognized. </exception>
		Public MustOverride Function newX509IssuerSerial(ByVal issuerName As String, ByVal serialNumber As System.Numerics.BigInteger) As X509IssuerSerial

		''' <summary>
		''' Indicates whether a specified feature is supported.
		''' </summary>
		''' <param name="feature"> the feature name (as an absolute URI) </param>
		''' <returns> <code>true</code> if the specified feature is supported,
		'''    <code>false</code> otherwise </returns>
		''' <exception cref="NullPointerException"> if <code>feature</code> is <code>null</code> </exception>
		Public MustOverride Function isFeatureSupported(ByVal feature As String) As Boolean

		''' <summary>
		''' Returns a reference to the <code>URIDereferencer</code> that is used by
		''' default to dereference URIs in <seealso cref="RetrievalMethod"/> objects.
		''' </summary>
		''' <returns> a reference to the default <code>URIDereferencer</code> </returns>
		Public MustOverride ReadOnly Property uRIDereferencer As javax.xml.crypto.URIDereferencer

		''' <summary>
		''' Unmarshals a new <code>KeyInfo</code> instance from a
		''' mechanism-specific <code>XMLStructure</code> (ex: <seealso cref="DOMStructure"/>)
		''' instance.
		''' </summary>
		''' <param name="xmlStructure"> a mechanism-specific XML structure from which to
		'''   unmarshal the keyinfo from </param>
		''' <returns> the <code>KeyInfo</code> </returns>
		''' <exception cref="NullPointerException"> if <code>xmlStructure</code> is
		'''   <code>null</code> </exception>
		''' <exception cref="ClassCastException"> if the type of <code>xmlStructure</code> is
		'''   inappropriate for this factory </exception>
		''' <exception cref="MarshalException"> if an unrecoverable exception occurs during
		'''   unmarshalling </exception>
		Public MustOverride Function unmarshalKeyInfo(ByVal xmlStructure As javax.xml.crypto.XMLStructure) As KeyInfo
	End Class

End Namespace