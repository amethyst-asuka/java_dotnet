Imports System.Collections
Imports javax.xml.crypto.dsig.spec
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
' * $Id: XMLSignatureFactory.java,v 1.14 2005/09/15 14:29:01 mullan Exp $
' 
Namespace javax.xml.crypto.dsig




	''' <summary>
	''' A factory for creating <seealso cref="XMLSignature"/> objects from scratch or
	''' for unmarshalling an <code>XMLSignature</code> object from a corresponding
	''' XML representation.
	''' 
	''' <h2>XMLSignatureFactory Type</h2>
	''' 
	''' <p>Each instance of <code>XMLSignatureFactory</code> supports a specific
	''' XML mechanism type. To create an <code>XMLSignatureFactory</code>, call one
	''' of the static <seealso cref="#getInstance getInstance"/> methods, passing in the XML
	''' mechanism type desired, for example:
	''' 
	''' <blockquote><code>
	''' XMLSignatureFactory factory = XMLSignatureFactory.getInstance("DOM");
	''' </code></blockquote>
	''' 
	''' <p>The objects that this factory produces will be based
	''' on DOM and abide by the DOM interoperability requirements as defined in the
	''' <a href="../../../../../technotes/guides/security/xmldsig/overview.html#DOM Mechanism Requirements">
	''' DOM Mechanism Requirements</a> section of the API overview. See the
	''' <a href="../../../../../technotes/guides/security/xmldsig/overview.html#Service Provider">
	''' Service Providers</a> section of the API overview for a list of standard
	''' mechanism types.
	''' 
	''' <p><code>XMLSignatureFactory</code> implementations are registered and loaded
	''' using the <seealso cref="java.security.Provider"/> mechanism.
	''' For example, a service provider that supports the
	''' DOM mechanism would be specified in the <code>Provider</code> subclass as:
	''' <pre>
	'''     put("XMLSignatureFactory.DOM", "org.example.DOMXMLSignatureFactory");
	''' </pre>
	''' 
	''' <p>An implementation MUST minimally support the default mechanism type: DOM.
	''' 
	''' <p>Note that a caller must use the same <code>XMLSignatureFactory</code>
	''' instance to create the <code>XMLStructure</code>s of a particular
	''' <code>XMLSignature</code> that is to be generated. The behavior is
	''' undefined if <code>XMLStructure</code>s from different providers or
	''' different mechanism types are used together.
	''' 
	''' <p>Also, the <code>XMLStructure</code>s that are created by this factory
	''' may contain state specific to the <code>XMLSignature</code> and are not
	''' intended to be reusable.
	''' 
	''' <h2>Creating XMLSignatures from scratch</h2>
	''' 
	''' <p>Once the <code>XMLSignatureFactory</code> has been created, objects
	''' can be instantiated by calling the appropriate method. For example, a
	''' <seealso cref="Reference"/> instance may be created by invoking one of the
	''' <seealso cref="#newReference newReference"/> methods.
	''' 
	''' <h2>Unmarshalling XMLSignatures from XML</h2>
	''' 
	''' <p>Alternatively, an <code>XMLSignature</code> may be created from an
	''' existing XML representation by invoking the {@link #unmarshalXMLSignature
	''' unmarshalXMLSignature} method and passing it a mechanism-specific
	''' <seealso cref="XMLValidateContext"/> instance containing the XML content:
	''' 
	''' <pre>
	''' DOMValidateContext context = new DOMValidateContext(key, signatureElement);
	''' XMLSignature signature = factory.unmarshalXMLSignature(context);
	''' </pre>
	''' 
	''' Each <code>XMLSignatureFactory</code> must support the required
	''' <code>XMLValidateContext</code> types for that factory type, but may support
	''' others. A DOM <code>XMLSignatureFactory</code> must support {@link
	''' DOMValidateContext} objects.
	''' 
	''' <h2>Signing and marshalling XMLSignatures to XML</h2>
	''' 
	''' Each <code>XMLSignature</code> created by the factory can also be
	''' marshalled to an XML representation and signed, by invoking the
	''' <seealso cref="XMLSignature#sign sign"/> method of the
	''' <seealso cref="XMLSignature"/> object and passing it a mechanism-specific
	''' <seealso cref="XMLSignContext"/> object containing the signing key and
	''' marshalling parameters (see <seealso cref="DOMSignContext"/>).
	''' For example:
	''' 
	''' <pre>
	'''    DOMSignContext context = new DOMSignContext(privateKey, document);
	'''    signature.sign(context);
	''' </pre>
	''' 
	''' <b>Concurrent Access</b>
	''' <p>The static methods of this class are guaranteed to be thread-safe.
	''' Multiple threads may concurrently invoke the static methods defined in this
	''' class with no ill effects.
	''' 
	''' <p>However, this is not true for the non-static methods defined by this
	''' class. Unless otherwise documented by a specific provider, threads that
	''' need to access a single <code>XMLSignatureFactory</code> instance
	''' concurrently should synchronize amongst themselves and provide the
	''' necessary locking. Multiple threads each manipulating a different
	''' <code>XMLSignatureFactory</code> instance need not synchronize.
	''' 
	''' @author Sean Mullan
	''' @author JSR 105 Expert Group
	''' @since 1.6
	''' </summary>
	Public MustInherit Class XMLSignatureFactory

		Private mechanismType As String
		Private provider As java.security.Provider

		''' <summary>
		''' Default constructor, for invocation by subclasses.
		''' </summary>
		Protected Friend Sub New()
		End Sub

		''' <summary>
		''' Returns an <code>XMLSignatureFactory</code> that supports the
		''' specified XML processing mechanism and representation type (ex: "DOM").
		''' 
		''' <p>This method uses the standard JCA provider lookup mechanism to
		''' locate and instantiate an <code>XMLSignatureFactory</code>
		''' implementation of the desired mechanism type. It traverses the list of
		''' registered security <code>Provider</code>s, starting with the most
		''' preferred <code>Provider</code>.  A new <code>XMLSignatureFactory</code>
		''' object from the first <code>Provider</code> that supports the specified
		''' mechanism is returned.
		''' 
		''' <p>Note that the list of registered providers may be retrieved via
		''' the <seealso cref="Security#getProviders() Security.getProviders()"/> method.
		''' </summary>
		''' <param name="mechanismType"> the type of the XML processing mechanism and
		'''    representation. See the <a
		'''    href="../../../../../technotes/guides/security/xmldsig/overview.html#Service Provider">
		'''    Service Providers</a> section of the API overview for a list of
		'''    standard mechanism types. </param>
		''' <returns> a new <code>XMLSignatureFactory</code> </returns>
		''' <exception cref="NullPointerException"> if <code>mechanismType</code> is
		'''    <code>null</code> </exception>
		''' <exception cref="NoSuchMechanismException"> if no <code>Provider</code> supports an
		'''    <code>XMLSignatureFactory</code> implementation for the specified
		'''    mechanism </exception>
		''' <seealso cref= Provider </seealso>
		Public Shared Function getInstance(ByVal mechanismType As String) As XMLSignatureFactory
			If mechanismType Is Nothing Then Throw New NullPointerException("mechanismType cannot be null")
			Dim ___instance As sun.security.jca.GetInstance.Instance
			Try
				___instance = GetInstance.getInstance("XMLSignatureFactory", Nothing, mechanismType)
			Catch nsae As java.security.NoSuchAlgorithmException
				Throw New javax.xml.crypto.NoSuchMechanismException(nsae)
			End Try
			Dim factory As XMLSignatureFactory = CType(___instance.impl, XMLSignatureFactory)
			factory.mechanismType = mechanismType
			factory.provider = ___instance.provider
			Return factory
		End Function

		''' <summary>
		''' Returns an <code>XMLSignatureFactory</code> that supports the
		''' requested XML processing mechanism and representation type (ex: "DOM"),
		''' as supplied by the specified provider. Note that the specified
		''' <code>Provider</code> object does not have to be registered in the
		''' provider list.
		''' </summary>
		''' <param name="mechanismType"> the type of the XML processing mechanism and
		'''    representation. See the <a
		'''    href="../../../../../technotes/guides/security/xmldsig/overview.html#Service Provider">
		'''    Service Providers</a> section of the API overview for a list of
		'''    standard mechanism types. </param>
		''' <param name="provider"> the <code>Provider</code> object </param>
		''' <returns> a new <code>XMLSignatureFactory</code> </returns>
		''' <exception cref="NullPointerException"> if <code>provider</code> or
		'''    <code>mechanismType</code> is <code>null</code> </exception>
		''' <exception cref="NoSuchMechanismException"> if an <code>XMLSignatureFactory</code>
		'''   implementation for the specified mechanism is not available
		'''   from the specified <code>Provider</code> object </exception>
		''' <seealso cref= Provider </seealso>
		Public Shared Function getInstance(ByVal mechanismType As String, ByVal provider As java.security.Provider) As XMLSignatureFactory
			If mechanismType Is Nothing Then
				Throw New NullPointerException("mechanismType cannot be null")
			ElseIf provider Is Nothing Then
				Throw New NullPointerException("provider cannot be null")
			End If

			Dim ___instance As sun.security.jca.GetInstance.Instance
			Try
				___instance = GetInstance.getInstance("XMLSignatureFactory", Nothing, mechanismType, provider)
			Catch nsae As java.security.NoSuchAlgorithmException
				Throw New javax.xml.crypto.NoSuchMechanismException(nsae)
			End Try
			Dim factory As XMLSignatureFactory = CType(___instance.impl, XMLSignatureFactory)
			factory.mechanismType = mechanismType
			factory.provider = ___instance.provider
			Return factory
		End Function

		''' <summary>
		''' Returns an <code>XMLSignatureFactory</code> that supports the
		''' requested XML processing mechanism and representation type (ex: "DOM"),
		''' as supplied by the specified provider. The specified provider must be
		''' registered in the security provider list.
		''' 
		''' <p>Note that the list of registered providers may be retrieved via
		''' the <seealso cref="Security#getProviders() Security.getProviders()"/> method.
		''' </summary>
		''' <param name="mechanismType"> the type of the XML processing mechanism and
		'''    representation. See the <a
		'''    href="../../../../../technotes/guides/security/xmldsig/overview.html#Service Provider">
		'''    Service Providers</a> section of the API overview for a list of
		'''    standard mechanism types. </param>
		''' <param name="provider"> the string name of the provider </param>
		''' <returns> a new <code>XMLSignatureFactory</code> </returns>
		''' <exception cref="NoSuchProviderException"> if the specified provider is not
		'''    registered in the security provider list </exception>
		''' <exception cref="NullPointerException"> if <code>provider</code> or
		'''    <code>mechanismType</code> is <code>null</code> </exception>
		''' <exception cref="NoSuchMechanismException"> if an <code>XMLSignatureFactory</code>
		'''    implementation for the specified mechanism is not
		'''    available from the specified provider </exception>
		''' <seealso cref= Provider </seealso>
		Public Shared Function getInstance(ByVal mechanismType As String, ByVal provider As String) As XMLSignatureFactory
			If mechanismType Is Nothing Then
				Throw New NullPointerException("mechanismType cannot be null")
			ElseIf provider Is Nothing Then
				Throw New NullPointerException("provider cannot be null")
			ElseIf provider.Length = 0 Then
				Throw New java.security.NoSuchProviderException
			End If

			Dim ___instance As sun.security.jca.GetInstance.Instance
			Try
				___instance = GetInstance.getInstance("XMLSignatureFactory", Nothing, mechanismType, provider)
			Catch nsae As java.security.NoSuchAlgorithmException
				Throw New javax.xml.crypto.NoSuchMechanismException(nsae)
			End Try
			Dim factory As XMLSignatureFactory = CType(___instance.impl, XMLSignatureFactory)
			factory.mechanismType = mechanismType
			factory.provider = ___instance.provider
			Return factory
		End Function

		''' <summary>
		''' Returns an <code>XMLSignatureFactory</code> that supports the
		''' default XML processing mechanism and representation type ("DOM").
		''' 
		''' <p>This method uses the standard JCA provider lookup mechanism to
		''' locate and instantiate an <code>XMLSignatureFactory</code>
		''' implementation of the default mechanism type. It traverses the list of
		''' registered security <code>Provider</code>s, starting with the most
		''' preferred <code>Provider</code>.  A new <code>XMLSignatureFactory</code>
		''' object from the first <code>Provider</code> that supports the DOM
		''' mechanism is returned.
		''' 
		''' <p>Note that the list of registered providers may be retrieved via
		''' the <seealso cref="Security#getProviders() Security.getProviders()"/> method.
		''' </summary>
		''' <returns> a new <code>XMLSignatureFactory</code> </returns>
		''' <exception cref="NoSuchMechanismException"> if no <code>Provider</code> supports an
		'''    <code>XMLSignatureFactory</code> implementation for the DOM
		'''    mechanism </exception>
		''' <seealso cref= Provider </seealso>
		Public Property Shared instance As XMLSignatureFactory
			Get
				Return getInstance("DOM")
			End Get
		End Property

		''' <summary>
		''' Returns the type of the XML processing mechanism and representation
		''' supported by this <code>XMLSignatureFactory</code> (ex: "DOM").
		''' </summary>
		''' <returns> the XML processing mechanism type supported by this
		'''    <code>XMLSignatureFactory</code> </returns>
		Public Property mechanismType As String
			Get
				Return mechanismType
			End Get
		End Property

		''' <summary>
		''' Returns the provider of this <code>XMLSignatureFactory</code>.
		''' </summary>
		''' <returns> the provider of this <code>XMLSignatureFactory</code> </returns>
		Public Property provider As java.security.Provider
			Get
				Return provider
			End Get
		End Property

		''' <summary>
		''' Creates an <code>XMLSignature</code> and initializes it with the contents
		''' of the specified <code>SignedInfo</code> and <code>KeyInfo</code>
		''' objects.
		''' </summary>
		''' <param name="si"> the signed info </param>
		''' <param name="ki"> the key info (may be <code>null</code>) </param>
		''' <returns> an <code>XMLSignature</code> </returns>
		''' <exception cref="NullPointerException"> if <code>si</code> is <code>null</code> </exception>
		Public MustOverride Function newXMLSignature(ByVal si As SignedInfo, ByVal ki As javax.xml.crypto.dsig.keyinfo.KeyInfo) As XMLSignature

		''' <summary>
		''' Creates an <code>XMLSignature</code> and initializes it with the
		''' specified parameters.
		''' </summary>
		''' <param name="si"> the signed info </param>
		''' <param name="ki"> the key info (may be <code>null</code>) </param>
		''' <param name="objects"> a list of <seealso cref="XMLObject"/>s (may be empty or
		'''    <code>null</code>) </param>
		''' <param name="id"> the Id (may be <code>null</code>) </param>
		''' <param name="signatureValueId"> the SignatureValue Id (may be <code>null</code>) </param>
		''' <returns> an <code>XMLSignature</code> </returns>
		''' <exception cref="NullPointerException"> if <code>si</code> is <code>null</code> </exception>
		''' <exception cref="ClassCastException"> if any of the <code>objects</code> are not of
		'''    type <code>XMLObject</code> </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public MustOverride Function newXMLSignature(ByVal si As SignedInfo, ByVal ki As javax.xml.crypto.dsig.keyinfo.KeyInfo, ByVal objects As IList, ByVal id As String, ByVal signatureValueId As String) As XMLSignature

		''' <summary>
		''' Creates a <code>Reference</code> with the specified URI and digest
		''' method.
		''' </summary>
		''' <param name="uri"> the reference URI (may be <code>null</code>) </param>
		''' <param name="dm"> the digest method </param>
		''' <returns> a <code>Reference</code> </returns>
		''' <exception cref="IllegalArgumentException"> if <code>uri</code> is not RFC 2396
		'''    compliant </exception>
		''' <exception cref="NullPointerException"> if <code>dm</code> is <code>null</code> </exception>
		Public MustOverride Function newReference(ByVal uri As String, ByVal dm As DigestMethod) As Reference

		''' <summary>
		''' Creates a <code>Reference</code> with the specified parameters.
		''' </summary>
		''' <param name="uri"> the reference URI (may be <code>null</code>) </param>
		''' <param name="dm"> the digest method </param>
		''' <param name="transforms"> a list of <seealso cref="Transform"/>s. The list is defensively
		'''    copied to protect against subsequent modification. May be
		'''    <code>null</code> or empty. </param>
		''' <param name="type"> the reference type, as a URI (may be <code>null</code>) </param>
		''' <param name="id"> the reference ID (may be <code>null</code>) </param>
		''' <returns> a <code>Reference</code> </returns>
		''' <exception cref="ClassCastException"> if any of the <code>transforms</code> are
		'''    not of type <code>Transform</code> </exception>
		''' <exception cref="IllegalArgumentException"> if <code>uri</code> is not RFC 2396
		'''    compliant </exception>
		''' <exception cref="NullPointerException"> if <code>dm</code> is <code>null</code> </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public MustOverride Function newReference(ByVal uri As String, ByVal dm As DigestMethod, ByVal transforms As IList, ByVal type As String, ByVal id As String) As Reference

		''' <summary>
		''' Creates a <code>Reference</code> with the specified parameters and
		''' pre-calculated digest value.
		''' 
		''' <p>This method is useful when the digest value of a
		''' <code>Reference</code> has been previously computed. See for example,
		''' the
		''' <a href="http://www.oasis-open.org/committees/tc_home.php?wg_abbrev=dss">
		''' OASIS-DSS (Digital Signature Services)</a> specification.
		''' </summary>
		''' <param name="uri"> the reference URI (may be <code>null</code>) </param>
		''' <param name="dm"> the digest method </param>
		''' <param name="transforms"> a list of <seealso cref="Transform"/>s. The list is defensively
		'''    copied to protect against subsequent modification. May be
		'''    <code>null</code> or empty. </param>
		''' <param name="type"> the reference type, as a URI (may be <code>null</code>) </param>
		''' <param name="id"> the reference ID (may be <code>null</code>) </param>
		''' <param name="digestValue"> the digest value. The array is cloned to protect
		'''    against subsequent modification. </param>
		''' <returns> a <code>Reference</code> </returns>
		''' <exception cref="ClassCastException"> if any of the <code>transforms</code> are
		'''    not of type <code>Transform</code> </exception>
		''' <exception cref="IllegalArgumentException"> if <code>uri</code> is not RFC 2396
		'''    compliant </exception>
		''' <exception cref="NullPointerException"> if <code>dm</code> or
		'''    <code>digestValue</code> is <code>null</code> </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public MustOverride Function newReference(ByVal uri As String, ByVal dm As DigestMethod, ByVal transforms As IList, ByVal type As String, ByVal id As String, ByVal digestValue As SByte()) As Reference

		''' <summary>
		''' Creates a <code>Reference</code> with the specified parameters.
		''' 
		''' <p>This method is useful when a list of transforms have already been
		''' applied to the <code>Reference</code>. See for example,
		''' the
		''' <a href="http://www.oasis-open.org/committees/tc_home.php?wg_abbrev=dss">
		''' OASIS-DSS (Digital Signature Services)</a> specification.
		''' 
		''' <p>When an <code>XMLSignature</code> containing this reference is
		''' generated, the specified <code>transforms</code> (if non-null) are
		''' applied to the specified <code>result</code>. The
		''' <code>Transforms</code> element of the resulting <code>Reference</code>
		''' element is set to the concatenation of the
		''' <code>appliedTransforms</code> and <code>transforms</code>.
		''' </summary>
		''' <param name="uri"> the reference URI (may be <code>null</code>) </param>
		''' <param name="dm"> the digest method </param>
		''' <param name="appliedTransforms"> a list of <seealso cref="Transform"/>s that have
		'''    already been applied. The list is defensively
		'''    copied to protect against subsequent modification. The list must
		'''    contain at least one entry. </param>
		''' <param name="result"> the result of processing the sequence of
		'''    <code>appliedTransforms</code> </param>
		''' <param name="transforms"> a list of <seealso cref="Transform"/>s that are to be applied
		'''    when generating the signature. The list is defensively copied to
		'''    protect against subsequent modification. May be <code>null</code>
		'''    or empty. </param>
		''' <param name="type"> the reference type, as a URI (may be <code>null</code>) </param>
		''' <param name="id"> the reference ID (may be <code>null</code>) </param>
		''' <returns> a <code>Reference</code> </returns>
		''' <exception cref="ClassCastException"> if any of the transforms (in either list)
		'''    are not of type <code>Transform</code> </exception>
		''' <exception cref="IllegalArgumentException"> if <code>uri</code> is not RFC 2396
		'''    compliant or <code>appliedTransforms</code> is empty </exception>
		''' <exception cref="NullPointerException"> if <code>dm</code>,
		'''    <code>appliedTransforms</code> or <code>result</code> is
		'''    <code>null</code> </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public MustOverride Function newReference(ByVal uri As String, ByVal dm As DigestMethod, ByVal appliedTransforms As IList, ByVal result As javax.xml.crypto.Data, ByVal transforms As IList, ByVal type As String, ByVal id As String) As Reference

		''' <summary>
		''' Creates a <code>SignedInfo</code> with the specified canonicalization
		''' and signature methods, and list of one or more references.
		''' </summary>
		''' <param name="cm"> the canonicalization method </param>
		''' <param name="sm"> the signature method </param>
		''' <param name="references"> a list of one or more <seealso cref="Reference"/>s. The list is
		'''    defensively copied to protect against subsequent modification. </param>
		''' <returns> a <code>SignedInfo</code> </returns>
		''' <exception cref="ClassCastException"> if any of the references are not of
		'''    type <code>Reference</code> </exception>
		''' <exception cref="IllegalArgumentException"> if <code>references</code> is empty </exception>
		''' <exception cref="NullPointerException"> if any of the parameters
		'''    are <code>null</code> </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public MustOverride Function newSignedInfo(ByVal cm As CanonicalizationMethod, ByVal sm As SignatureMethod, ByVal references As IList) As SignedInfo

		''' <summary>
		''' Creates a <code>SignedInfo</code> with the specified parameters.
		''' </summary>
		''' <param name="cm"> the canonicalization method </param>
		''' <param name="sm"> the signature method </param>
		''' <param name="references"> a list of one or more <seealso cref="Reference"/>s. The list is
		'''    defensively copied to protect against subsequent modification. </param>
		''' <param name="id"> the id (may be <code>null</code>) </param>
		''' <returns> a <code>SignedInfo</code> </returns>
		''' <exception cref="ClassCastException"> if any of the references are not of
		'''    type <code>Reference</code> </exception>
		''' <exception cref="IllegalArgumentException"> if <code>references</code> is empty </exception>
		''' <exception cref="NullPointerException"> if <code>cm</code>, <code>sm</code>, or
		'''    <code>references</code> are <code>null</code> </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public MustOverride Function newSignedInfo(ByVal cm As CanonicalizationMethod, ByVal sm As SignatureMethod, ByVal references As IList, ByVal id As String) As SignedInfo

		' Object factory methods
		''' <summary>
		''' Creates an <code>XMLObject</code> from the specified parameters.
		''' </summary>
		''' <param name="content"> a list of <seealso cref="XMLStructure"/>s. The list
		'''    is defensively copied to protect against subsequent modification.
		'''    May be <code>null</code> or empty. </param>
		''' <param name="id"> the Id (may be <code>null</code>) </param>
		''' <param name="mimeType"> the mime type (may be <code>null</code>) </param>
		''' <param name="encoding"> the encoding (may be <code>null</code>) </param>
		''' <returns> an <code>XMLObject</code> </returns>
		''' <exception cref="ClassCastException"> if <code>content</code> contains any
		'''    entries that are not of type <seealso cref="XMLStructure"/> </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public MustOverride Function newXMLObject(ByVal content As IList, ByVal id As String, ByVal mimeType As String, ByVal encoding As String) As XMLObject

		''' <summary>
		''' Creates a <code>Manifest</code> containing the specified
		''' list of <seealso cref="Reference"/>s.
		''' </summary>
		''' <param name="references"> a list of one or more <code>Reference</code>s. The list
		'''    is defensively copied to protect against subsequent modification. </param>
		''' <returns> a <code>Manifest</code> </returns>
		''' <exception cref="NullPointerException"> if <code>references</code> is
		'''    <code>null</code> </exception>
		''' <exception cref="IllegalArgumentException"> if <code>references</code> is empty </exception>
		''' <exception cref="ClassCastException"> if <code>references</code> contains any
		'''    entries that are not of type <seealso cref="Reference"/> </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public MustOverride Function newManifest(ByVal references As IList) As Manifest

		''' <summary>
		''' Creates a <code>Manifest</code> containing the specified
		''' list of <seealso cref="Reference"/>s and optional id.
		''' </summary>
		''' <param name="references"> a list of one or more <code>Reference</code>s. The list
		'''    is defensively copied to protect against subsequent modification. </param>
		''' <param name="id"> the id (may be <code>null</code>) </param>
		''' <returns> a <code>Manifest</code> </returns>
		''' <exception cref="NullPointerException"> if <code>references</code> is
		'''    <code>null</code> </exception>
		''' <exception cref="IllegalArgumentException"> if <code>references</code> is empty </exception>
		''' <exception cref="ClassCastException"> if <code>references</code> contains any
		'''    entries that are not of type <seealso cref="Reference"/> </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public MustOverride Function newManifest(ByVal references As IList, ByVal id As String) As Manifest

		''' <summary>
		''' Creates a <code>SignatureProperty</code> containing the specified
		''' list of <seealso cref="XMLStructure"/>s, target URI and optional id.
		''' </summary>
		''' <param name="content"> a list of one or more <code>XMLStructure</code>s. The list
		'''    is defensively copied to protect against subsequent modification. </param>
		''' <param name="target"> the target URI of the Signature that this property applies
		'''    to </param>
		''' <param name="id"> the id (may be <code>null</code>) </param>
		''' <returns> a <code>SignatureProperty</code> </returns>
		''' <exception cref="NullPointerException"> if <code>content</code> or
		'''    <code>target</code> is <code>null</code> </exception>
		''' <exception cref="IllegalArgumentException"> if <code>content</code> is empty </exception>
		''' <exception cref="ClassCastException"> if <code>content</code> contains any
		'''    entries that are not of type <seealso cref="XMLStructure"/> </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public MustOverride Function newSignatureProperty(ByVal content As IList, ByVal target As String, ByVal id As String) As SignatureProperty

		''' <summary>
		''' Creates a <code>SignatureProperties</code> containing the specified
		''' list of <seealso cref="SignatureProperty"/>s and optional id.
		''' </summary>
		''' <param name="properties"> a list of one or more <code>SignatureProperty</code>s.
		'''    The list is defensively copied to protect against subsequent
		'''    modification. </param>
		''' <param name="id"> the id (may be <code>null</code>) </param>
		''' <returns> a <code>SignatureProperties</code> </returns>
		''' <exception cref="NullPointerException"> if <code>properties</code>
		'''    is <code>null</code> </exception>
		''' <exception cref="IllegalArgumentException"> if <code>properties</code> is empty </exception>
		''' <exception cref="ClassCastException"> if <code>properties</code> contains any
		'''    entries that are not of type <seealso cref="SignatureProperty"/> </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public MustOverride Function newSignatureProperties(ByVal properties As IList, ByVal id As String) As SignatureProperties

		' Algorithm factory methods
		''' <summary>
		''' Creates a <code>DigestMethod</code> for the specified algorithm URI
		''' and parameters.
		''' </summary>
		''' <param name="algorithm"> the URI identifying the digest algorithm </param>
		''' <param name="params"> algorithm-specific digest parameters (may be
		'''    <code>null</code>) </param>
		''' <returns> the <code>DigestMethod</code> </returns>
		''' <exception cref="InvalidAlgorithmParameterException"> if the specified parameters
		'''    are inappropriate for the requested algorithm </exception>
		''' <exception cref="NoSuchAlgorithmException"> if an implementation of the
		'''    specified algorithm cannot be found </exception>
		''' <exception cref="NullPointerException"> if <code>algorithm</code> is
		'''    <code>null</code> </exception>
		Public MustOverride Function newDigestMethod(ByVal algorithm As String, ByVal params As DigestMethodParameterSpec) As DigestMethod

		''' <summary>
		''' Creates a <code>SignatureMethod</code> for the specified algorithm URI
		''' and parameters.
		''' </summary>
		''' <param name="algorithm"> the URI identifying the signature algorithm </param>
		''' <param name="params"> algorithm-specific signature parameters (may be
		'''    <code>null</code>) </param>
		''' <returns> the <code>SignatureMethod</code> </returns>
		''' <exception cref="InvalidAlgorithmParameterException"> if the specified parameters
		'''    are inappropriate for the requested algorithm </exception>
		''' <exception cref="NoSuchAlgorithmException"> if an implementation of the
		'''    specified algorithm cannot be found </exception>
		''' <exception cref="NullPointerException"> if <code>algorithm</code> is
		'''    <code>null</code> </exception>
		Public MustOverride Function newSignatureMethod(ByVal algorithm As String, ByVal params As SignatureMethodParameterSpec) As SignatureMethod

		''' <summary>
		''' Creates a <code>Transform</code> for the specified algorithm URI
		''' and parameters.
		''' </summary>
		''' <param name="algorithm"> the URI identifying the transform algorithm </param>
		''' <param name="params"> algorithm-specific transform parameters (may be
		'''    <code>null</code>) </param>
		''' <returns> the <code>Transform</code> </returns>
		''' <exception cref="InvalidAlgorithmParameterException"> if the specified parameters
		'''    are inappropriate for the requested algorithm </exception>
		''' <exception cref="NoSuchAlgorithmException"> if an implementation of the
		'''    specified algorithm cannot be found </exception>
		''' <exception cref="NullPointerException"> if <code>algorithm</code> is
		'''    <code>null</code> </exception>
		Public MustOverride Function newTransform(ByVal algorithm As String, ByVal params As TransformParameterSpec) As Transform

		''' <summary>
		''' Creates a <code>Transform</code> for the specified algorithm URI
		''' and parameters. The parameters are specified as a mechanism-specific
		''' <code>XMLStructure</code> (ex: <seealso cref="DOMStructure"/>). This method is
		''' useful when the parameters are in XML form or there is no standard
		''' class for specifying the parameters.
		''' </summary>
		''' <param name="algorithm"> the URI identifying the transform algorithm </param>
		''' <param name="params"> a mechanism-specific XML structure from which to
		'''   unmarshal the parameters from (may be <code>null</code> if
		'''   not required or optional) </param>
		''' <returns> the <code>Transform</code> </returns>
		''' <exception cref="ClassCastException"> if the type of <code>params</code> is
		'''   inappropriate for this <code>XMLSignatureFactory</code> </exception>
		''' <exception cref="InvalidAlgorithmParameterException"> if the specified parameters
		'''    are inappropriate for the requested algorithm </exception>
		''' <exception cref="NoSuchAlgorithmException"> if an implementation of the
		'''    specified algorithm cannot be found </exception>
		''' <exception cref="NullPointerException"> if <code>algorithm</code> is
		'''    <code>null</code> </exception>
		Public MustOverride Function newTransform(ByVal algorithm As String, ByVal params As javax.xml.crypto.XMLStructure) As Transform

		''' <summary>
		''' Creates a <code>CanonicalizationMethod</code> for the specified
		''' algorithm URI and parameters.
		''' </summary>
		''' <param name="algorithm"> the URI identifying the canonicalization algorithm </param>
		''' <param name="params"> algorithm-specific canonicalization parameters (may be
		'''    <code>null</code>) </param>
		''' <returns> the <code>CanonicalizationMethod</code> </returns>
		''' <exception cref="InvalidAlgorithmParameterException"> if the specified parameters
		'''    are inappropriate for the requested algorithm </exception>
		''' <exception cref="NoSuchAlgorithmException"> if an implementation of the
		'''    specified algorithm cannot be found </exception>
		''' <exception cref="NullPointerException"> if <code>algorithm</code> is
		'''    <code>null</code> </exception>
		Public MustOverride Function newCanonicalizationMethod(ByVal algorithm As String, ByVal params As C14NMethodParameterSpec) As CanonicalizationMethod

		''' <summary>
		''' Creates a <code>CanonicalizationMethod</code> for the specified
		''' algorithm URI and parameters. The parameters are specified as a
		''' mechanism-specific <code>XMLStructure</code> (ex: <seealso cref="DOMStructure"/>).
		''' This method is useful when the parameters are in XML form or there is
		''' no standard class for specifying the parameters.
		''' </summary>
		''' <param name="algorithm"> the URI identifying the canonicalization algorithm </param>
		''' <param name="params"> a mechanism-specific XML structure from which to
		'''   unmarshal the parameters from (may be <code>null</code> if
		'''   not required or optional) </param>
		''' <returns> the <code>CanonicalizationMethod</code> </returns>
		''' <exception cref="ClassCastException"> if the type of <code>params</code> is
		'''   inappropriate for this <code>XMLSignatureFactory</code> </exception>
		''' <exception cref="InvalidAlgorithmParameterException"> if the specified parameters
		'''    are inappropriate for the requested algorithm </exception>
		''' <exception cref="NoSuchAlgorithmException"> if an implementation of the
		'''    specified algorithm cannot be found </exception>
		''' <exception cref="NullPointerException"> if <code>algorithm</code> is
		'''    <code>null</code> </exception>
		Public MustOverride Function newCanonicalizationMethod(ByVal algorithm As String, ByVal params As javax.xml.crypto.XMLStructure) As CanonicalizationMethod

		''' <summary>
		''' Returns a <code>KeyInfoFactory</code> that creates <code>KeyInfo</code>
		''' objects. The returned <code>KeyInfoFactory</code> has the same
		''' mechanism type and provider as this <code>XMLSignatureFactory</code>.
		''' </summary>
		''' <returns> a <code>KeyInfoFactory</code> </returns>
		''' <exception cref="NoSuchMechanismException"> if a <code>KeyFactory</code>
		'''    implementation with the same mechanism type and provider
		'''    is not available </exception>
		Public Property keyInfoFactory As javax.xml.crypto.dsig.keyinfo.KeyInfoFactory
			Get
				Return javax.xml.crypto.dsig.keyinfo.KeyInfoFactory.getInstance(mechanismType, provider)
			End Get
		End Property

		''' <summary>
		''' Unmarshals a new <code>XMLSignature</code> instance from a
		''' mechanism-specific <code>XMLValidateContext</code> instance.
		''' </summary>
		''' <param name="context"> a mechanism-specific context from which to unmarshal the
		'''    signature from </param>
		''' <returns> the <code>XMLSignature</code> </returns>
		''' <exception cref="NullPointerException"> if <code>context</code> is
		'''    <code>null</code> </exception>
		''' <exception cref="ClassCastException"> if the type of <code>context</code> is
		'''    inappropriate for this factory </exception>
		''' <exception cref="MarshalException"> if an unrecoverable exception occurs
		'''    during unmarshalling </exception>
		Public MustOverride Function unmarshalXMLSignature(ByVal context As XMLValidateContext) As XMLSignature

		''' <summary>
		''' Unmarshals a new <code>XMLSignature</code> instance from a
		''' mechanism-specific <code>XMLStructure</code> instance.
		''' This method is useful if you only want to unmarshal (and not
		''' validate) an <code>XMLSignature</code>.
		''' </summary>
		''' <param name="xmlStructure"> a mechanism-specific XML structure from which to
		'''    unmarshal the signature from </param>
		''' <returns> the <code>XMLSignature</code> </returns>
		''' <exception cref="NullPointerException"> if <code>xmlStructure</code> is
		'''    <code>null</code> </exception>
		''' <exception cref="ClassCastException"> if the type of <code>xmlStructure</code> is
		'''    inappropriate for this factory </exception>
		''' <exception cref="MarshalException"> if an unrecoverable exception occurs
		'''    during unmarshalling </exception>
		Public MustOverride Function unmarshalXMLSignature(ByVal xmlStructure As javax.xml.crypto.XMLStructure) As XMLSignature

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
		''' default to dereference URIs in <seealso cref="Reference"/> objects.
		''' </summary>
		''' <returns> a reference to the default <code>URIDereferencer</code> (never
		'''    <code>null</code>) </returns>
		Public MustOverride ReadOnly Property uRIDereferencer As javax.xml.crypto.URIDereferencer
	End Class

End Namespace