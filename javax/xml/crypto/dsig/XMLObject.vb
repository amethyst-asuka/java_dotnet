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
' * $Id: XMLObject.java,v 1.5 2005/05/10 16:03:48 mullan Exp $
' 
Namespace javax.xml.crypto.dsig


	''' <summary>
	''' A representation of the XML <code>Object</code> element as defined in
	''' the <a href="http://www.w3.org/TR/xmldsig-core/">
	''' W3C Recommendation for XML-Signature Syntax and Processing</a>.
	''' An <code>XMLObject</code> may contain any data and may include optional
	''' MIME type, ID, and encoding attributes. The XML Schema Definition is
	''' defined as:
	''' 
	''' <pre><code>
	''' &lt;element name="Object" type="ds:ObjectType"/&gt;
	''' &lt;complexType name="ObjectType" mixed="true"&gt;
	'''   &lt;sequence minOccurs="0" maxOccurs="unbounded"&gt;
	'''     &lt;any namespace="##any" processContents="lax"/&gt;
	'''   &lt;/sequence&gt;
	'''   &lt;attribute name="Id" type="ID" use="optional"/&gt;
	'''   &lt;attribute name="MimeType" type="string" use="optional"/&gt;
	'''   &lt;attribute name="Encoding" type="anyURI" use="optional"/&gt;
	''' &lt;/complexType&gt;
	''' </code></pre>
	''' 
	''' A <code>XMLObject</code> instance may be created by invoking the
	''' <seealso cref="XMLSignatureFactory#newXMLObject newXMLObject"/> method of the
	''' <seealso cref="XMLSignatureFactory"/> class; for example:
	''' 
	''' <pre>
	'''   XMLSignatureFactory fac = XMLSignatureFactory.getInstance("DOM");
	'''   List content = Collections.singletonList(fac.newManifest(references)));
	'''   XMLObject object = factory.newXMLObject(content, "object-1", null, null);
	''' </pre>
	''' 
	''' <p>Note that this class is named <code>XMLObject</code> rather than
	''' <code>Object</code> to avoid naming clashes with the existing
	''' <seealso cref="java.lang.Object java.lang.Object"/> class.
	''' 
	''' @author Sean Mullan
	''' @author JSR 105 Expert Group
	''' @author Joyce L. Leung
	''' @since 1.6 </summary>
	''' <seealso cref= XMLSignatureFactory#newXMLObject(List, String, String, String) </seealso>
	Public Interface XMLObject
		Inherits javax.xml.crypto.XMLStructure

		''' <summary>
		''' URI that identifies the <code>Object</code> element (this can be
		''' specified as the value of the <code>type</code> parameter of the
		''' <seealso cref="Reference"/> class to identify the referent's type).
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		final static String TYPE = "http://www.w3.org/2000/09/xmldsig#Object";

		''' <summary>
		''' Returns an {@link java.util.Collections#unmodifiableList unmodifiable
		''' list} of <seealso cref="XMLStructure"/>s contained in this <code>XMLObject</code>,
		''' which represent elements from any namespace.
		''' 
		''' <p>If there is a public subclass representing the type of
		''' <code>XMLStructure</code>, it is returned as an instance of that class
		''' (ex: a <code>SignatureProperties</code> element would be returned
		''' as an instance of <seealso cref="javax.xml.crypto.dsig.SignatureProperties"/>).
		''' </summary>
		''' <returns> an unmodifiable list of <code>XMLStructure</code>s (may be empty
		'''    but never <code>null</code>) </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		ReadOnly Property content As IList

		''' <summary>
		''' Returns the Id of this <code>XMLObject</code>.
		''' </summary>
		''' <returns> the Id (or <code>null</code> if not specified) </returns>
		ReadOnly Property id As String

		''' <summary>
		''' Returns the mime type of this <code>XMLObject</code>. The
		''' mime type is an optional attribute which describes the data within this
		''' <code>XMLObject</code> (independent of its encoding).
		''' </summary>
		''' <returns> the mime type (or <code>null</code> if not specified) </returns>
		ReadOnly Property mimeType As String

		''' <summary>
		''' Returns the encoding URI of this <code>XMLObject</code>. The encoding
		''' URI identifies the method by which the object is encoded.
		''' </summary>
		''' <returns> the encoding URI (or <code>null</code> if not specified) </returns>
		ReadOnly Property encoding As String
	End Interface

End Namespace