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
' * $Id: DigestMethod.java,v 1.6 2005/05/10 16:03:46 mullan Exp $
' 
Namespace javax.xml.crypto.dsig


	''' <summary>
	''' A representation of the XML <code>DigestMethod</code> element as
	''' defined in the <a href="http://www.w3.org/TR/xmldsig-core/">
	''' W3C Recommendation for XML-Signature Syntax and Processing</a>.
	''' The XML Schema Definition is defined as:
	''' <p>
	''' <pre>
	'''   &lt;element name="DigestMethod" type="ds:DigestMethodType"/&gt;
	'''     &lt;complexType name="DigestMethodType" mixed="true"&gt;
	'''       &lt;sequence&gt;
	'''         &lt;any namespace="##any" minOccurs="0" maxOccurs="unbounded"/&gt;
	'''           &lt;!-- (0,unbounded) elements from (1,1) namespace --&gt;
	'''       &lt;/sequence&gt;
	'''       &lt;attribute name="Algorithm" type="anyURI" use="required"/&gt;
	'''     &lt;/complexType&gt;
	''' </pre>
	''' 
	''' A <code>DigestMethod</code> instance may be created by invoking the
	''' <seealso cref="XMLSignatureFactory#newDigestMethod newDigestMethod"/> method
	''' of the <seealso cref="XMLSignatureFactory"/> class.
	''' 
	''' @author Sean Mullan
	''' @author JSR 105 Expert Group
	''' @since 1.6 </summary>
	''' <seealso cref= XMLSignatureFactory#newDigestMethod(String, DigestMethodParameterSpec) </seealso>
	Public Interface DigestMethod
		Inherits javax.xml.crypto.XMLStructure, javax.xml.crypto.AlgorithmMethod

		''' <summary>
		''' The <a href="http://www.w3.org/2000/09/xmldsig#sha1">
		''' SHA1</a> digest method algorithm URI.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		static final String SHA1 = "http://www.w3.org/2000/09/xmldsig#sha1";

		''' <summary>
		''' The <a href="http://www.w3.org/2001/04/xmlenc#sha256">
		''' SHA256</a> digest method algorithm URI.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		static final String SHA256 = "http://www.w3.org/2001/04/xmlenc#sha256";

		''' <summary>
		''' The <a href="http://www.w3.org/2001/04/xmlenc#sha512">
		''' SHA512</a> digest method algorithm URI.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		static final String SHA512 = "http://www.w3.org/2001/04/xmlenc#sha512";

		''' <summary>
		''' The <a href="http://www.w3.org/2001/04/xmlenc#ripemd160">
		''' RIPEMD-160</a> digest method algorithm URI.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		static final String RIPEMD160 = "http://www.w3.org/2001/04/xmlenc#ripemd160";

		''' <summary>
		''' Returns the algorithm-specific input parameters associated with this
		''' <code>DigestMethod</code>.
		''' 
		''' <p>The returned parameters can be typecast to a {@link
		''' DigestMethodParameterSpec} object.
		''' </summary>
		''' <returns> the algorithm-specific parameters (may be <code>null</code> if
		'''    not specified) </returns>
		ReadOnly Property parameterSpec As java.security.spec.AlgorithmParameterSpec
	End Interface

End Namespace