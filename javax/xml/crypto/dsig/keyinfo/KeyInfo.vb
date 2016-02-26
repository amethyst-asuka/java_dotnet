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
' * $Id: KeyInfo.java,v 1.7 2005/05/10 16:35:34 mullan Exp $
' 
Namespace javax.xml.crypto.dsig.keyinfo


	''' <summary>
	''' A representation of the XML <code>KeyInfo</code> element as defined in
	''' the <a href="http://www.w3.org/TR/xmldsig-core/">
	''' W3C Recommendation for XML-Signature Syntax and Processing</a>.
	''' A <code>KeyInfo</code> contains a list of <seealso cref="XMLStructure"/>s, each of
	''' which contain information that enables the recipient(s) to obtain the key
	''' needed to validate an XML signature. The XML Schema Definition is defined as:
	''' 
	''' <pre>
	''' &lt;element name="KeyInfo" type="ds:KeyInfoType"/&gt;
	''' &lt;complexType name="KeyInfoType" mixed="true"&gt;
	'''   &lt;choice maxOccurs="unbounded"&gt;
	'''     &lt;element ref="ds:KeyName"/&gt;
	'''     &lt;element ref="ds:KeyValue"/&gt;
	'''     &lt;element ref="ds:RetrievalMethod"/&gt;
	'''     &lt;element ref="ds:X509Data"/&gt;
	'''     &lt;element ref="ds:PGPData"/&gt;
	'''     &lt;element ref="ds:SPKIData"/&gt;
	'''     &lt;element ref="ds:MgmtData"/&gt;
	'''     &lt;any processContents="lax" namespace="##other"/&gt;
	'''     &lt;!-- (1,1) elements from (0,unbounded) namespaces --&gt;
	'''   &lt;/choice&gt;
	'''   &lt;attribute name="Id" type="ID" use="optional"/&gt;
	''' &lt;/complexType&gt;
	''' </pre>
	''' 
	''' A <code>KeyInfo</code> instance may be created by invoking one of the
	''' <seealso cref="KeyInfoFactory#newKeyInfo newKeyInfo"/> methods of the
	''' <seealso cref="KeyInfoFactory"/> class, and passing it a list of one or more
	''' <code>XMLStructure</code>s and an optional id parameter;
	''' for example:
	''' <pre>
	'''   KeyInfoFactory factory = KeyInfoFactory.getInstance("DOM");
	'''   KeyInfo keyInfo = factory.newKeyInfo
	'''      (Collections.singletonList(factory.newKeyName("Alice"), "keyinfo-1"));
	''' </pre>
	''' 
	''' <p><code>KeyInfo</code> objects can also be marshalled to XML by invoking
	''' the <seealso cref="#marshal marshal"/> method.
	''' 
	''' @author Sean Mullan
	''' @author JSR 105 Expert Group
	''' @since 1.6 </summary>
	''' <seealso cref= KeyInfoFactory#newKeyInfo(List) </seealso>
	''' <seealso cref= KeyInfoFactory#newKeyInfo(List, String) </seealso>
	Public Interface KeyInfo
		Inherits javax.xml.crypto.XMLStructure

		''' <summary>
		''' Returns an {@link java.util.Collections#unmodifiableList unmodifiable
		''' list} containing the key information. Each entry of the list is
		''' an <seealso cref="XMLStructure"/>.
		''' 
		''' <p>If there is a public subclass representing the type of
		''' <code>XMLStructure</code>, it is returned as an instance of that
		''' class (ex: an <code>X509Data</code> element would be returned as an
		''' instance of <seealso cref="javax.xml.crypto.dsig.keyinfo.X509Data"/>).
		''' </summary>
		''' <returns> an unmodifiable list of one or more <code>XMLStructure</code>s
		'''    in this <code>KeyInfo</code>. Never returns <code>null</code> or an
		'''    empty list. </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		ReadOnly Property content As IList

		''' <summary>
		''' Return the optional Id attribute of this <code>KeyInfo</code>, which
		''' may be useful for referencing this <code>KeyInfo</code> from other
		''' XML structures.
		''' </summary>
		''' <returns> the Id attribute of this <code>KeyInfo</code> (may be
		'''    <code>null</code> if not specified) </returns>
		ReadOnly Property id As String

		''' <summary>
		''' Marshals the key info to XML.
		''' </summary>
		''' <param name="parent"> a mechanism-specific structure containing the parent node
		'''    that the marshalled key info will be appended to </param>
		''' <param name="context"> the <code>XMLCryptoContext</code> containing additional
		'''    context (may be null if not applicable) </param>
		''' <exception cref="ClassCastException"> if the type of <code>parent</code> or
		'''    <code>context</code> is not compatible with this key info </exception>
		''' <exception cref="MarshalException"> if the key info cannot be marshalled </exception>
		''' <exception cref="NullPointerException"> if <code>parent</code> is <code>null</code> </exception>
		Sub marshal(ByVal parent As javax.xml.crypto.XMLStructure, ByVal context As javax.xml.crypto.XMLCryptoContext)
	End Interface

End Namespace