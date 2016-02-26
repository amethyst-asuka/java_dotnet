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
' * $Id: Transform.java,v 1.5 2005/05/10 16:03:48 mullan Exp $
' 
Namespace javax.xml.crypto.dsig


	''' <summary>
	''' A representation of the XML <code>Transform</code> element as
	''' defined in the <a href="http://www.w3.org/TR/xmldsig-core/">
	''' W3C Recommendation for XML-Signature Syntax and Processing</a>.
	''' The XML Schema Definition is defined as:
	''' 
	''' <pre>
	''' &lt;element name="Transform" type="ds:TransformType"/&gt;
	'''   &lt;complexType name="TransformType" mixed="true"&gt;
	'''     &lt;choice minOccurs="0" maxOccurs="unbounded"&gt;
	'''       &lt;any namespace="##other" processContents="lax"/&gt;
	'''       &lt;!-- (1,1) elements from (0,unbounded) namespaces --&gt;
	'''       &lt;element name="XPath" type="string"/&gt;
	'''     &lt;/choice&gt;
	'''     &lt;attribute name="Algorithm" type="anyURI" use="required"/&gt;
	'''   &lt;/complexType&gt;
	''' </pre>
	''' 
	''' A <code>Transform</code> instance may be created by invoking the
	''' <seealso cref="XMLSignatureFactory#newTransform newTransform"/> method
	''' of the <seealso cref="XMLSignatureFactory"/> class.
	''' 
	''' @author Sean Mullan
	''' @author JSR 105 Expert Group
	''' @since 1.6 </summary>
	''' <seealso cref= XMLSignatureFactory#newTransform(String, TransformParameterSpec) </seealso>
	Public Interface Transform
		Inherits javax.xml.crypto.XMLStructure, javax.xml.crypto.AlgorithmMethod

		''' <summary>
		''' The <a href="http://www.w3.org/2000/09/xmldsig#base64">Base64</a>
		''' transform algorithm URI.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		final static String BASE64 = "http://www.w3.org/2000/09/xmldsig#base64";

		''' <summary>
		''' The <a href="http://www.w3.org/2000/09/xmldsig#enveloped-signature">
		''' Enveloped Signature</a> transform algorithm URI.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		final static String ENVELOPED = "http://www.w3.org/2000/09/xmldsig#enveloped-signature";

		''' <summary>
		''' The <a href="http://www.w3.org/TR/1999/REC-xpath-19991116">XPath</a>
		''' transform algorithm URI.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		final static String XPATH = "http://www.w3.org/TR/1999/REC-xpath-19991116";

		''' <summary>
		''' The <a href="http://www.w3.org/2002/06/xmldsig-filter2">
		''' XPath Filter 2</a> transform algorithm URI.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		final static String XPATH2 = "http://www.w3.org/2002/06/xmldsig-filter2";

		''' <summary>
		''' The <a href="http://www.w3.org/TR/1999/REC-xslt-19991116">XSLT</a>
		''' transform algorithm URI.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		final static String XSLT = "http://www.w3.org/TR/1999/REC-xslt-19991116";

		''' <summary>
		''' Returns the algorithm-specific input parameters associated with this
		''' <code>Transform</code>.
		''' <p>
		''' The returned parameters can be typecast to a
		''' <seealso cref="TransformParameterSpec"/> object.
		''' </summary>
		''' <returns> the algorithm-specific input parameters (may be <code>null</code>
		'''    if not specified) </returns>
		ReadOnly Property parameterSpec As java.security.spec.AlgorithmParameterSpec

		''' <summary>
		''' Transforms the specified data using the underlying transform algorithm.
		''' </summary>
		''' <param name="data"> the data to be transformed </param>
		''' <param name="context"> the <code>XMLCryptoContext</code> containing
		'''    additional context (may be <code>null</code> if not applicable) </param>
		''' <returns> the transformed data </returns>
		''' <exception cref="NullPointerException"> if <code>data</code> is <code>null</code> </exception>
		''' <exception cref="TransformException"> if an error occurs while executing the
		'''    transform </exception>
		Function transform(ByVal data As javax.xml.crypto.Data, ByVal context As javax.xml.crypto.XMLCryptoContext) As javax.xml.crypto.Data

		''' <summary>
		''' Transforms the specified data using the underlying transform algorithm.
		''' If the output of this transform is an <code>OctetStreamData</code>, then
		''' this method returns <code>null</code> and the bytes are written to the
		''' specified <code>OutputStream</code>. Otherwise, the
		''' <code>OutputStream</code> is ignored and the method behaves as if
		''' <seealso cref="#transform(Data, XMLCryptoContext)"/> were invoked.
		''' </summary>
		''' <param name="data"> the data to be transformed </param>
		''' <param name="context"> the <code>XMLCryptoContext</code> containing
		'''    additional context (may be <code>null</code> if not applicable) </param>
		''' <param name="os"> the <code>OutputStream</code> that should be used to write
		'''    the transformed data to </param>
		''' <returns> the transformed data (or <code>null</code> if the data was
		'''    written to the <code>OutputStream</code> parameter) </returns>
		''' <exception cref="NullPointerException"> if <code>data</code> or <code>os</code>
		'''    is <code>null</code> </exception>
		''' <exception cref="TransformException"> if an error occurs while executing the
		'''    transform </exception>
		Function transform(ByVal data As javax.xml.crypto.Data, ByVal context As javax.xml.crypto.XMLCryptoContext, ByVal os As java.io.OutputStream) As javax.xml.crypto.Data
	End Interface

End Namespace