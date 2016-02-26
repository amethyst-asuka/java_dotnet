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
' * $Id: SignatureMethod.java,v 1.5 2005/05/10 16:03:46 mullan Exp $
' 
Namespace javax.xml.crypto.dsig


	''' <summary>
	''' A representation of the XML <code>SignatureMethod</code> element
	''' as defined in the <a href="http://www.w3.org/TR/xmldsig-core/">
	''' W3C Recommendation for XML-Signature Syntax and Processing</a>.
	''' The XML Schema Definition is defined as:
	''' <p>
	''' <pre>
	'''   &lt;element name="SignatureMethod" type="ds:SignatureMethodType"/&gt;
	'''     &lt;complexType name="SignatureMethodType" mixed="true"&gt;
	'''       &lt;sequence&gt;
	'''         &lt;element name="HMACOutputLength" minOccurs="0" type="ds:HMACOutputLengthType"/&gt;
	'''         &lt;any namespace="##any" minOccurs="0" maxOccurs="unbounded"/&gt;
	'''           &lt;!-- (0,unbounded) elements from (1,1) namespace --&gt;
	'''       &lt;/sequence&gt;
	'''       &lt;attribute name="Algorithm" type="anyURI" use="required"/&gt;
	'''     &lt;/complexType&gt;
	''' </pre>
	''' 
	''' A <code>SignatureMethod</code> instance may be created by invoking the
	''' <seealso cref="XMLSignatureFactory#newSignatureMethod newSignatureMethod"/> method
	''' of the <seealso cref="XMLSignatureFactory"/> class.
	''' 
	''' @author Sean Mullan
	''' @author JSR 105 Expert Group
	''' @since 1.6 </summary>
	''' <seealso cref= XMLSignatureFactory#newSignatureMethod(String, SignatureMethodParameterSpec) </seealso>
	Public Interface SignatureMethod
		Inherits javax.xml.crypto.XMLStructure, javax.xml.crypto.AlgorithmMethod

		''' <summary>
		''' The <a href="http://www.w3.org/2000/09/xmldsig#dsa-sha1">DSAwithSHA1</a>
		''' (DSS) signature method algorithm URI.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		static final String DSA_SHA1 = "http://www.w3.org/2000/09/xmldsig#dsa-sha1";

		''' <summary>
		''' The <a href="http://www.w3.org/2000/09/xmldsig#rsa-sha1">RSAwithSHA1</a>
		''' (PKCS #1) signature method algorithm URI.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		static final String RSA_SHA1 = "http://www.w3.org/2000/09/xmldsig#rsa-sha1";

		''' <summary>
		''' The <a href="http://www.w3.org/2000/09/xmldsig#hmac-sha1">HMAC-SHA1</a>
		''' MAC signature method algorithm URI
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		static final String HMAC_SHA1 = "http://www.w3.org/2000/09/xmldsig#hmac-sha1";

		''' <summary>
		''' Returns the algorithm-specific input parameters of this
		''' <code>SignatureMethod</code>.
		''' 
		''' <p>The returned parameters can be typecast to a {@link
		''' SignatureMethodParameterSpec} object.
		''' </summary>
		''' <returns> the algorithm-specific input parameters of this
		'''    <code>SignatureMethod</code> (may be <code>null</code> if not
		'''    specified) </returns>
		ReadOnly Property parameterSpec As java.security.spec.AlgorithmParameterSpec
	End Interface

End Namespace