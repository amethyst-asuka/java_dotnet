'
' * Copyright (c) 2005, Oracle and/or its affiliates. All rights reserved.
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
' * $Id: KeyValue.java,v 1.4 2005/05/10 16:35:35 mullan Exp $
' 
Namespace javax.xml.crypto.dsig.keyinfo


	''' <summary>
	''' A representation of the XML <code>KeyValue</code> element as defined
	''' in the <a href="http://www.w3.org/TR/xmldsig-core/">
	''' W3C Recommendation for XML-Signature Syntax and Processing</a>. A
	''' <code>KeyValue</code> object contains a single public key that may be
	''' useful in validating the signature. The XML schema definition is defined as:
	''' 
	''' <pre>
	'''    &lt;element name="KeyValue" type="ds:KeyValueType"/&gt;
	'''    &lt;complexType name="KeyValueType" mixed="true"&gt;
	'''      &lt;choice&gt;
	'''        &lt;element ref="ds:DSAKeyValue"/&gt;
	'''        &lt;element ref="ds:RSAKeyValue"/&gt;
	'''        &lt;any namespace="##other" processContents="lax"/&gt;
	'''      &lt;/choice&gt;
	'''    &lt;/complexType&gt;
	''' 
	'''    &lt;element name="DSAKeyValue" type="ds:DSAKeyValueType"/&gt;
	'''    &lt;complexType name="DSAKeyValueType"&gt;
	'''      &lt;sequence&gt;
	'''        &lt;sequence minOccurs="0"&gt;
	'''          &lt;element name="P" type="ds:CryptoBinary"/&gt;
	'''          &lt;element name="Q" type="ds:CryptoBinary"/&gt;
	'''        &lt;/sequence&gt;
	'''        &lt;element name="G" type="ds:CryptoBinary" minOccurs="0"/&gt;
	'''        &lt;element name="Y" type="ds:CryptoBinary"/&gt;
	'''        &lt;element name="J" type="ds:CryptoBinary" minOccurs="0"/&gt;
	'''        &lt;sequence minOccurs="0"&gt;
	'''          &lt;element name="Seed" type="ds:CryptoBinary"/&gt;
	'''          &lt;element name="PgenCounter" type="ds:CryptoBinary"/&gt;
	'''        &lt;/sequence&gt;
	'''      &lt;/sequence&gt;
	'''    &lt;/complexType&gt;
	''' 
	'''    &lt;element name="RSAKeyValue" type="ds:RSAKeyValueType"/&gt;
	'''    &lt;complexType name="RSAKeyValueType"&gt;
	'''      &lt;sequence&gt;
	'''        &lt;element name="Modulus" type="ds:CryptoBinary"/&gt;
	'''        &lt;element name="Exponent" type="ds:CryptoBinary"/&gt;
	'''      &lt;/sequence&gt;
	'''    &lt;/complexType&gt;
	''' </pre>
	''' A <code>KeyValue</code> instance may be created by invoking the
	''' <seealso cref="KeyInfoFactory#newKeyValue newKeyValue"/> method of the
	''' <seealso cref="KeyInfoFactory"/> class, and passing it a {@link
	''' java.security.PublicKey} representing the value of the public key. Here is
	''' an example of creating a <code>KeyValue</code> from a <seealso cref="DSAPublicKey"/>
	''' of a <seealso cref="java.security.cert.Certificate"/> stored in a
	''' <seealso cref="java.security.KeyStore"/>:
	''' <pre>
	''' KeyStore keyStore = KeyStore.getInstance(KeyStore.getDefaultType());
	''' PublicKey dsaPublicKey = keyStore.getCertificate("myDSASigningCert").getPublicKey();
	''' KeyInfoFactory factory = KeyInfoFactory.getInstance("DOM");
	''' KeyValue keyValue = factory.newKeyValue(dsaPublicKey);
	''' </pre>
	''' 
	''' This class returns the <code>DSAKeyValue</code> and
	''' <code>RSAKeyValue</code> elements as objects of type
	''' <seealso cref="DSAPublicKey"/> and <seealso cref="RSAPublicKey"/>, respectively. Note that not
	''' all of the fields in the schema are accessible as parameters of these
	''' types.
	''' 
	''' @author Sean Mullan
	''' @author JSR 105 Expert Group
	''' @since 1.6 </summary>
	''' <seealso cref= KeyInfoFactory#newKeyValue(PublicKey) </seealso>
	Public Interface KeyValue
		Inherits javax.xml.crypto.XMLStructure

		''' <summary>
		''' URI identifying the DSA KeyValue KeyInfo type:
		''' http://www.w3.org/2000/09/xmldsig#DSAKeyValue. This can be specified as
		''' the value of the <code>type</code> parameter of the
		''' <seealso cref="RetrievalMethod"/> class to describe a remote
		''' <code>DSAKeyValue</code> structure.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		final static String DSA_TYPE = "http://www.w3.org/2000/09/xmldsig#DSAKeyValue";

		''' <summary>
		''' URI identifying the RSA KeyValue KeyInfo type:
		''' http://www.w3.org/2000/09/xmldsig#RSAKeyValue. This can be specified as
		''' the value of the <code>type</code> parameter of the
		''' <seealso cref="RetrievalMethod"/> class to describe a remote
		''' <code>RSAKeyValue</code> structure.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		final static String RSA_TYPE = "http://www.w3.org/2000/09/xmldsig#RSAKeyValue";

		''' <summary>
		''' Returns the public key of this <code>KeyValue</code>.
		''' </summary>
		''' <returns> the public key of this <code>KeyValue</code> </returns>
		''' <exception cref="KeyException"> if this <code>KeyValue</code> cannot be converted
		'''    to a <code>PublicKey</code> </exception>
		ReadOnly Property publicKey As java.security.PublicKey
	End Interface

End Namespace