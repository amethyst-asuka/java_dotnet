Imports System.Collections

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
' * ===========================================================================
' *
' * (C) Copyright IBM Corp. 2003 All Rights Reserved.
' *
' * ===========================================================================
' 
'
' * $Id: XMLSignature.java,v 1.10 2005/05/10 16:03:48 mullan Exp $
' 
Namespace javax.xml.crypto.dsig


	''' <summary>
	''' A representation of the XML <code>Signature</code> element as
	''' defined in the <a href="http://www.w3.org/TR/xmldsig-core/">
	''' W3C Recommendation for XML-Signature Syntax and Processing</a>.
	''' This class contains methods for signing and validating XML signatures
	''' with behavior as defined by the W3C specification. The XML Schema Definition
	''' is defined as:
	''' <pre><code>
	''' &lt;element name="Signature" type="ds:SignatureType"/&gt;
	''' &lt;complexType name="SignatureType"&gt;
	'''    &lt;sequence&gt;
	'''      &lt;element ref="ds:SignedInfo"/&gt;
	'''      &lt;element ref="ds:SignatureValue"/&gt;
	'''      &lt;element ref="ds:KeyInfo" minOccurs="0"/&gt;
	'''      &lt;element ref="ds:Object" minOccurs="0" maxOccurs="unbounded"/&gt;
	'''    &lt;/sequence&gt;
	'''    &lt;attribute name="Id" type="ID" use="optional"/&gt;
	''' &lt;/complexType&gt;
	''' </code></pre>
	''' <p>
	''' An <code>XMLSignature</code> instance may be created by invoking one of the
	''' <seealso cref="XMLSignatureFactory#newXMLSignature newXMLSignature"/> methods of the
	''' <seealso cref="XMLSignatureFactory"/> class.
	''' 
	''' <p>If the contents of the underlying document containing the
	''' <code>XMLSignature</code> are subsequently modified, the behavior is
	''' undefined.
	''' 
	''' <p>Note that this class is named <code>XMLSignature</code> rather than
	''' <code>Signature</code> to avoid naming clashes with the existing
	''' <seealso cref="Signature java.security.Signature"/> class.
	''' </summary>
	''' <seealso cref= XMLSignatureFactory#newXMLSignature(SignedInfo, KeyInfo) </seealso>
	''' <seealso cref= XMLSignatureFactory#newXMLSignature(SignedInfo, KeyInfo, List, String, String)
	''' @author Joyce L. Leung
	''' @author Sean Mullan
	''' @author Erwin van der Koogh
	''' @author JSR 105 Expert Group
	''' @since 1.6 </seealso>
	Public Interface XMLSignature
		Inherits javax.xml.crypto.XMLStructure

		''' <summary>
		''' The XML Namespace URI of the W3C Recommendation for XML-Signature
		''' Syntax and Processing.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		final static String XMLNS = "http://www.w3.org/2000/09/xmldsig#";

		''' <summary>
		''' Validates the signature according to the
		''' <a href="http://www.w3.org/TR/xmldsig-core/#sec-CoreValidation">
		''' core validation processing rules</a>. This method validates the
		''' signature using the existing state, it does not unmarshal and
		''' reinitialize the contents of the <code>XMLSignature</code> using the
		''' location information specified in the context.
		''' 
		''' <p>This method only validates the signature the first time it is
		''' invoked. On subsequent invocations, it returns a cached result.
		''' </summary>
		''' <param name="validateContext"> the validating context </param>
		''' <returns> <code>true</code> if the signature passed core validation,
		'''    otherwise <code>false</code> </returns>
		''' <exception cref="ClassCastException"> if the type of <code>validateContext</code>
		'''    is not compatible with this <code>XMLSignature</code> </exception>
		''' <exception cref="NullPointerException"> if <code>validateContext</code> is
		'''    <code>null</code> </exception>
		''' <exception cref="XMLSignatureException"> if an unexpected error occurs during
		'''    validation that prevented the validation operation from completing </exception>
		Function validate(ByVal validateContext As XMLValidateContext) As Boolean

		''' <summary>
		''' Returns the key info of this <code>XMLSignature</code>.
		''' </summary>
		''' <returns> the key info (may be <code>null</code> if not specified) </returns>
		ReadOnly Property keyInfo As javax.xml.crypto.dsig.keyinfo.KeyInfo

		''' <summary>
		''' Returns the signed info of this <code>XMLSignature</code>.
		''' </summary>
		''' <returns> the signed info (never <code>null</code>) </returns>
		ReadOnly Property signedInfo As SignedInfo

		''' <summary>
		''' Returns an {@link java.util.Collections#unmodifiableList unmodifiable
		''' list} of <seealso cref="XMLObject"/>s contained in this <code>XMLSignature</code>.
		''' </summary>
		''' <returns> an unmodifiable list of <code>XMLObject</code>s (may be empty
		'''    but never <code>null</code>) </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		ReadOnly Property objects As IList

		''' <summary>
		''' Returns the optional Id of this <code>XMLSignature</code>.
		''' </summary>
		''' <returns> the Id (may be <code>null</code> if not specified) </returns>
		ReadOnly Property id As String

		''' <summary>
		''' Returns the signature value of this <code>XMLSignature</code>.
		''' </summary>
		''' <returns> the signature value </returns>
		ReadOnly Property signatureValue As SignatureValue

		''' <summary>
		''' Signs this <code>XMLSignature</code>.
		''' 
		''' <p>If this method throws an exception, this <code>XMLSignature</code> and
		''' the <code>signContext</code> parameter will be left in the state that
		''' it was in prior to the invocation.
		''' </summary>
		''' <param name="signContext"> the signing context </param>
		''' <exception cref="ClassCastException"> if the type of <code>signContext</code> is
		'''    not compatible with this <code>XMLSignature</code> </exception>
		''' <exception cref="NullPointerException"> if <code>signContext</code> is
		'''    <code>null</code> </exception>
		''' <exception cref="MarshalException"> if an exception occurs while marshalling </exception>
		''' <exception cref="XMLSignatureException"> if an unexpected exception occurs while
		'''    generating the signature </exception>
		Sub sign(ByVal signContext As XMLSignContext)

		''' <summary>
		''' Returns the result of the <seealso cref="KeySelector"/>, if specified, after
		''' this <code>XMLSignature</code> has been signed or validated.
		''' </summary>
		''' <returns> the key selector result, or <code>null</code> if a key
		'''    selector has not been specified or this <code>XMLSignature</code>
		'''    has not been signed or validated </returns>
		ReadOnly Property keySelectorResult As javax.xml.crypto.KeySelectorResult

		''' <summary>
		''' A representation of the XML <code>SignatureValue</code> element as
		''' defined in the <a href="http://www.w3.org/TR/xmldsig-core/">
		''' W3C Recommendation for XML-Signature Syntax and Processing</a>.
		''' The XML Schema Definition is defined as:
		''' <p>
		''' <pre>
		'''   &lt;element name="SignatureValue" type="ds:SignatureValueType"/&gt;
		'''     &lt;complexType name="SignatureValueType"&gt;
		'''       &lt;simpleContent&gt;
		'''         &lt;extension base="base64Binary"&gt;
		'''           &lt;attribute name="Id" type="ID" use="optional"/&gt;
		'''         &lt;/extension&gt;
		'''       &lt;/simpleContent&gt;
		'''     &lt;/complexType&gt;
		''' </pre>
		''' 
		''' @author Sean Mullan
		''' @author JSR 105 Expert Group
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interface inner types are not converted:
'		public interface SignatureValue extends javax.xml.crypto.XMLStructure
	'	{
	'		''' <summary>
	'		''' Returns the optional <code>Id</code> attribute of this
	'		''' <code>SignatureValue</code>, which permits this element to be
	'		''' referenced from elsewhere.
	'		''' </summary>
	'		''' <returns> the <code>Id</code> attribute (may be <code>null</code> if
	'		'''    not specified) </returns>
	'		String getId();
	'
	'		''' <summary>
	'		''' Returns the signature value of this <code>SignatureValue</code>.
	'		''' </summary>
	'		''' <returns> the signature value (may be <code>null</code> if the
	'		'''    <code>XMLSignature</code> has not been signed yet). Each
	'		'''    invocation of this method returns a new clone of the array to
	'		'''    prevent subsequent modification. </returns>
	'		byte[] getValue();
	'
	'		''' <summary>
	'		''' Validates the signature value. This method performs a
	'		''' cryptographic validation of the signature calculated over the
	'		''' <code>SignedInfo</code> of the <code>XMLSignature</code>.
	'		''' 
	'		''' <p>This method only validates the signature the first
	'		''' time it is invoked. On subsequent invocations, it returns a cached
	'		''' result.
	'		''' </summary>
	'		''' <returns> <code>true</code> if the signature was
	'		'''    validated successfully; <code>false</code> otherwise </returns>
	'		''' <param name="validateContext"> the validating context </param>
	'		''' <exception cref="NullPointerException"> if <code>validateContext</code> is
	'		'''    <code>null</code> </exception>
	'		''' <exception cref="XMLSignatureException"> if an unexpected exception occurs while
	'		'''    validating the signature </exception>
	'		boolean validate(XMLValidateContext validateContext) throws XMLSignatureException;
	'	}
	End Interface

End Namespace