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
' * $Id: SignedInfo.java,v 1.7 2005/05/10 16:03:47 mullan Exp $
' 
Namespace javax.xml.crypto.dsig


	''' <summary>
	''' An representation of the XML <code>SignedInfo</code> element as
	''' defined in the <a href="http://www.w3.org/TR/xmldsig-core/">
	''' W3C Recommendation for XML-Signature Syntax and Processing</a>.
	''' The XML Schema Definition is defined as:
	''' <pre><code>
	''' &lt;element name="SignedInfo" type="ds:SignedInfoType"/&gt;
	''' &lt;complexType name="SignedInfoType"&gt;
	'''   &lt;sequence&gt;
	'''     &lt;element ref="ds:CanonicalizationMethod"/&gt;
	'''     &lt;element ref="ds:SignatureMethod"/&gt;
	'''     &lt;element ref="ds:Reference" maxOccurs="unbounded"/&gt;
	'''   &lt;/sequence&gt;
	'''   &lt;attribute name="Id" type="ID" use="optional"/&gt;
	''' &lt;/complexType&gt;
	''' </code></pre>
	''' 
	''' A <code>SignedInfo</code> instance may be created by invoking one of the
	''' <seealso cref="XMLSignatureFactory#newSignedInfo newSignedInfo"/> methods of the
	''' <seealso cref="XMLSignatureFactory"/> class.
	''' 
	''' @author Sean Mullan
	''' @author JSR 105 Expert Group
	''' @since 1.6 </summary>
	''' <seealso cref= XMLSignatureFactory#newSignedInfo(CanonicalizationMethod, SignatureMethod, List) </seealso>
	''' <seealso cref= XMLSignatureFactory#newSignedInfo(CanonicalizationMethod, SignatureMethod, List, String) </seealso>
	Public Interface SignedInfo
		Inherits javax.xml.crypto.XMLStructure

		''' <summary>
		''' Returns the canonicalization method of this <code>SignedInfo</code>.
		''' </summary>
		''' <returns> the canonicalization method </returns>
		ReadOnly Property canonicalizationMethod As CanonicalizationMethod

		''' <summary>
		''' Returns the signature method of this <code>SignedInfo</code>.
		''' </summary>
		''' <returns> the signature method </returns>
		ReadOnly Property signatureMethod As SignatureMethod

		''' <summary>
		''' Returns an {@link java.util.Collections#unmodifiableList
		''' unmodifiable list} of one or more <seealso cref="Reference"/>s.
		''' </summary>
		''' <returns> an unmodifiable list of one or more <seealso cref="Reference"/>s </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		ReadOnly Property references As IList

		''' <summary>
		''' Returns the optional <code>Id</code> attribute of this
		''' <code>SignedInfo</code>.
		''' </summary>
		''' <returns> the id (may be <code>null</code> if not specified) </returns>
		ReadOnly Property id As String

		''' <summary>
		''' Returns the canonicalized signed info bytes after a signing or
		''' validation operation. This method is useful for debugging.
		''' </summary>
		''' <returns> an <code>InputStream</code> containing the canonicalized bytes,
		'''    or <code>null</code> if this <code>SignedInfo</code> has not been
		'''    signed or validated yet </returns>
		ReadOnly Property canonicalizedData As java.io.InputStream
	End Interface

End Namespace