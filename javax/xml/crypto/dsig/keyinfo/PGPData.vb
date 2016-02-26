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
' * $Id: PGPData.java,v 1.4 2005/05/10 16:35:35 mullan Exp $
' 
Namespace javax.xml.crypto.dsig.keyinfo


	''' <summary>
	''' A representation of the XML <code>PGPData</code> element as defined in
	''' the <a href="http://www.w3.org/TR/xmldsig-core/">
	''' W3C Recommendation for XML-Signature Syntax and Processing</a>. A
	''' <code>PGPData</code> object is used to convey information related to
	''' PGP public key pairs and signatures on such keys. The XML Schema Definition
	''' is defined as:
	''' 
	''' <pre>
	'''    &lt;element name="PGPData" type="ds:PGPDataType"/&gt;
	'''    &lt;complexType name="PGPDataType"&gt;
	'''      &lt;choice&gt;
	'''        &lt;sequence&gt;
	'''          &lt;element name="PGPKeyID" type="base64Binary"/&gt;
	'''          &lt;element name="PGPKeyPacket" type="base64Binary" minOccurs="0"/&gt;
	'''          &lt;any namespace="##other" processContents="lax" minOccurs="0"
	'''           maxOccurs="unbounded"/&gt;
	'''        &lt;/sequence&gt;
	'''        &lt;sequence&gt;
	'''          &lt;element name="PGPKeyPacket" type="base64Binary"/&gt;
	'''          &lt;any namespace="##other" processContents="lax" minOccurs="0"
	'''           maxOccurs="unbounded"/&gt;
	'''        &lt;/sequence&gt;
	'''      &lt;/choice&gt;
	'''    &lt;/complexType&gt;
	''' </pre>
	''' 
	''' A <code>PGPData</code> instance may be created by invoking one of the
	''' <seealso cref="KeyInfoFactory#newPGPData newPGPData"/> methods of the {@link
	''' KeyInfoFactory} class, and passing it
	''' <code>byte</code> arrays representing the contents of the PGP public key
	''' identifier and/or PGP key material packet, and an optional list of
	''' elements from an external namespace.
	''' 
	''' @author Sean Mullan
	''' @author JSR 105 Expert Group
	''' @since 1.6 </summary>
	''' <seealso cref= KeyInfoFactory#newPGPData(byte[]) </seealso>
	''' <seealso cref= KeyInfoFactory#newPGPData(byte[], byte[], List) </seealso>
	''' <seealso cref= KeyInfoFactory#newPGPData(byte[], List) </seealso>
	Public Interface PGPData
		Inherits javax.xml.crypto.XMLStructure

		''' <summary>
		''' URI identifying the PGPData KeyInfo type:
		''' http://www.w3.org/2000/09/xmldsig#PGPData. This can be specified as the
		''' value of the <code>type</code> parameter of the <seealso cref="RetrievalMethod"/>
		''' class to describe a remote <code>PGPData</code> structure.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		final static String TYPE = "http://www.w3.org/2000/09/xmldsig#PGPData";

		''' <summary>
		''' Returns the PGP public key identifier of this <code>PGPData</code> as
		''' defined in <a href="http://www.ietf.org/rfc/rfc2440.txt">RFC 2440</a>,
		''' section 11.2.
		''' </summary>
		''' <returns> the PGP public key identifier (may be <code>null</code> if
		'''    not specified). Each invocation of this method returns a new clone
		'''    to protect against subsequent modification. </returns>
		ReadOnly Property keyId As SByte()

		''' <summary>
		''' Returns the PGP key material packet of this <code>PGPData</code> as
		''' defined in <a href="http://www.ietf.org/rfc/rfc2440.txt">RFC 2440</a>,
		''' section 5.5.
		''' </summary>
		''' <returns> the PGP key material packet (may be <code>null</code> if not
		'''    specified). Each invocation of this method returns a new clone to
		'''    protect against subsequent modification. </returns>
		ReadOnly Property keyPacket As SByte()

		''' <summary>
		''' Returns an <seealso cref="Collections#unmodifiableList unmodifiable list"/>
		''' of <seealso cref="XMLStructure"/>s representing elements from an external
		''' namespace.
		''' </summary>
		''' <returns> an unmodifiable list of <code>XMLStructure</code>s (may be
		'''    empty, but never <code>null</code>) </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		ReadOnly Property externalElements As IList
	End Interface

End Namespace